using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Profiling;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zuy.TenebrousRecursion.Component;
using Zuy.TenebrousRecursion.Mono;

public class InstanceRenderPassFeature : ScriptableRendererFeature
{
    private InstanceRenderPass _pass;

    public override void Create()
    {
        _pass = new InstanceRenderPass
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques
        };
        Debug.Log("InstanceRenderPassFeature created.");
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        _pass?.Dispose();
    }
}

public class InstanceRenderPass : ScriptableRenderPass
{
    private const int BatchSize = 1023;

    private static readonly ProfilerMarker ExecuteMarker = new ProfilerMarker("ZuyExecute");
    private static readonly ProfilerMarker RenderMarker = new ProfilerMarker("ZuyRender");
    private static readonly ProfilerMarker SubmitMarker = new ProfilerMarker("ZuySubmit");

    private EntityManager _entityManager;
    private EntityQuery _enemyQuery;
    private List<InstanceRendererData> _renderData;
    private NativeList<Entity> _batcher;
    private CommandBuffer _commandBuffer;
    private NativeList<LocalToWorld> _localToWorlds;
    private MaterialPropertyBlock _propertyBlock;
    private Vector4[] _offsetScaleArray;
    private Vector2[] _spriteOffsetArray;
    private Matrix4x4[] _matrices;
    private NativeArray<Entity> _entities;

    private bool _isInitialized;
    private int _batchCount;
    private InstanceRendererData _currentData;

    public InstanceRenderPass()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
        {
            Debug.LogWarning("DefaultGameObjectInjectionWorld is null.");
            return;
        }

        _entityManager = world.EntityManager;
        if (_entityManager == null)
        {
            Debug.LogWarning("EntityManager is null.");
            return;
        }

        SetupEntityQuery();
        InitializeLists();
        InitializeArrays();

        _isInitialized = true;
        Debug.Log("InstanceRenderPass initialized.");
    }

    private void SetupEntityQuery()
    {
        _enemyQuery = _entityManager.CreateEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadWrite<InstanceRendererData>(),
                ComponentType.ReadWrite<Enemy>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<MaterialOverrideOffset>(),
                ComponentType.ReadOnly<SpriteFrameElement>()
            },
            Options = EntityQueryOptions.FilterWriteGroup
        });
    }

    private void InitializeLists()
    {
        _renderData = new List<InstanceRendererData>();
        _batcher = new NativeList<Entity>(1000, Allocator.Persistent);
        _localToWorlds = new NativeList<LocalToWorld>(1000, Allocator.Persistent);
    }

    private void InitializeArrays()
    {
        _matrices = new Matrix4x4[BatchSize];
        for (int i = 0; i < BatchSize; i++)
        {
            _matrices[i] = Matrix4x4.identity;
        }

        _offsetScaleArray = new Vector4[BatchSize];
        _spriteOffsetArray = new Vector2[BatchSize];
        _propertyBlock = new MaterialPropertyBlock();
    }

    public void Dispose()
    {
        _batcher.Dispose();
        _localToWorlds.Dispose();
        _entities.Dispose();
        Debug.Log("InstanceRenderPass disposed.");
    }

    [System.Obsolete]
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        using (ExecuteMarker.Auto())
        {
            if (!_isInitialized)
            {
                return;
            }

            int entityCount = _enemyQuery.CalculateEntityCount();
            if (entityCount == 0)
                return;

            PrepareForExecution(entityCount);
            Render();
            context.ExecuteCommandBuffer(_commandBuffer);
        }
    }

    private void PrepareForExecution(int entityCount)
    {
        EnsureEntityArraySize(entityCount);
        UpdateRenderData();
        PrepareCommandBuffer();
    }

    private void EnsureEntityArraySize(int entityCount)
    {
        if (!_entities.IsCreated || _entities.Length != entityCount)
        {
            _entities.Dispose();
            _entities = _enemyQuery.ToEntityArray(Allocator.Persistent);
            Debug.Log($"Entities fetched: {_entities.Length}");
        }
    }

    private void UpdateRenderData()
    {
        _renderData.Clear();
        _entityManager.GetAllUniqueSharedComponentsManaged(_renderData);
    }

    private void PrepareCommandBuffer()
    {
        if (_commandBuffer == null)
        {
            _commandBuffer = CommandBufferPool.Get("InstanceRenderPass");
            _commandBuffer.name = "Instance";
        }
        else
        {
            _commandBuffer.Clear();
        }

        _commandBuffer.ClearRenderTarget(true, true, Color.clear);
        _batcher.Clear();
    }

    private void Render()
    {
        using (RenderMarker.Auto())
        {
            if (_renderData.Count <= 1)
            {
                Debug.LogWarning("Insufficient render data available.");
                return;
            }

            _currentData = _renderData[1];

            using var localToWorlds = _enemyQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
            using var materialOverrides = _enemyQuery.ToComponentDataArray<MaterialOverrideOffset>(Allocator.Temp);

            if (localToWorlds.Length == 0 || materialOverrides.Length == 0)
            {
                Debug.LogWarning("LocalToWorld or MaterialOverrideOffset arrays are empty.");
                return;
            }

            ProcessEntities(localToWorlds, materialOverrides);
            Submit();
        }
    }

    private void ProcessEntities(NativeArray<LocalToWorld> localToWorlds, NativeArray<MaterialOverrideOffset> materialOverrides)
    {
        int count = Mathf.Min(_entities.Length, BatchSize);
        _batchCount = count;
        Debug.Log($"Batch Count: {_batchCount}");

        for (int i = 0; i < count; i++)
        {
            ProcessEntity(i, localToWorlds[i], materialOverrides[i], _entities[i]);
        }
    }

    private void ProcessEntity(int index, LocalToWorld localToWorld, MaterialOverrideOffset materialOverride, Entity entity)
    {
        _matrices[index] = localToWorld.Value;
        _offsetScaleArray[index] = new Vector4(materialOverride.Offset.x, materialOverride.Offset.y,
                                               materialOverride.Scale.x, materialOverride.Scale.y);

        var spriteFrameBuffer = _entityManager.GetBuffer<SpriteFrameElement>(entity);
        _spriteOffsetArray[index] = spriteFrameBuffer.Length > 0 ? spriteFrameBuffer[0].offset : Vector2.zero;

        if (spriteFrameBuffer.Length == 0)
        {
            Debug.LogWarning($"No SpriteFrameElement found for entity {entity}.");
        }
    }

    private void Submit()
    {
        using (SubmitMarker.Auto())
        {
            if (_batchCount == 0)
            {
                Debug.LogWarning("No batches to submit.");
                return;
            }

            if (_currentData.Mesh == null || _currentData.Material == null)
            {
                Debug.LogWarning("Mesh or Material is null.");
                return;
            }

            SetMaterialProperties();
            Debug.Log($"Drawing Mesh with {_batchCount} instances.");

            _commandBuffer.DrawMeshInstanced(_currentData.Mesh, _currentData.SubMesh, _currentData.Material, 0, _matrices, _batchCount, _propertyBlock);
            _batchCount = 0;
        }
    }

    private void SetMaterialProperties()
    {
        var spriteOffsetList = new List<Vector4>(_batchCount);
        for (int i = 0; i < _batchCount; i++)
        {
            spriteOffsetList.Add(new Vector4(_spriteOffsetArray[i].x, _spriteOffsetArray[i].y, 0, 0));
        }

        _propertyBlock.SetVectorArray("_OffsetXYScaleZW", _offsetScaleArray);
        _propertyBlock.SetVectorArray("_SpriteOffset", spriteOffsetList);
        _propertyBlock.SetTexture("_MainTex", ConfigManager.Instance.GetEnemeyConfigByType(0).textureSheet);
    }
}
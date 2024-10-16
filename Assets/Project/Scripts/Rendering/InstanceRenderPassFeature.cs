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

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }

    public override void Create()
    {
        _pass = new InstanceRenderPass();

        // Configures where the render pass should be injected.
        _pass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    protected override void Dispose(bool disposing)
    {
        if (_pass != null)
            _pass.Dispose();
    }
}

class InstanceRenderPass : ScriptableRenderPass
{
    private EntityManager _entityManager;
    private EntityQuery _enemyQuery;

    private List<InstanceRendererData> _renderData;
    private NativeList<Entity> _batcher;
    private CommandBuffer _commandBuffer;
    private int batchCount;
    private InstanceRendererData data;
    private Matrix4x4[] matrix;
    private bool _inited;

    private const int batchSize = 1023;

    // Profiling markers
    private static readonly ProfilerMarker ExecuteMarker = new ProfilerMarker("ZuyExecute");
    private static readonly ProfilerMarker RenderMarker = new ProfilerMarker("ZuyRender");
    private static readonly ProfilerMarker BatchMarker = new ProfilerMarker("ZuyBatch");
    private static readonly ProfilerMarker SubmitMarker = new ProfilerMarker("ZuySubmit");

    private bool _shouldUpdateRenderData = true;

    private NativeList<LocalToWorld> localToWorlds;
    private MaterialPropertyBlock propertyBlock;
    private Vector4[] offsetScaleArray;
    private Vector2[] spriteOffsetArray;

    NativeArray<Entity> entities;
    public InstanceRenderPass()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
            return;

        _entityManager = world.EntityManager;
        if (_entityManager == null)
            return;

        _enemyQuery = _entityManager.CreateEntityQuery(new EntityQueryDesc()
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

        _renderData = new List<InstanceRendererData>();
        _entityManager.GetAllUniqueSharedComponentsManaged(_renderData);
        _batcher = new NativeList<Entity>(1000, Allocator.Persistent);
        localToWorlds = new NativeList<LocalToWorld>(1000, Allocator.Persistent);

        matrix = new Matrix4x4[1023];
        for (int i = 0; i < 1023; i++)
        {
            matrix[i] = Matrix4x4.identity;
        }

        offsetScaleArray = new Vector4[batchSize];
        spriteOffsetArray = new Vector2[batchSize];
        propertyBlock = new MaterialPropertyBlock();
        _inited = true;
    }

    public void Dispose()
    {
        if (_batcher.IsCreated) _batcher.Dispose();
        if (localToWorlds.IsCreated) localToWorlds.Dispose();
        if (entities.IsCreated) entities.Dispose();
    }

    NativeArray<LocalToWorld> a;
    NativeArray<MaterialOverrideOffset> b;


    [System.Obsolete]
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        using (ExecuteMarker.Auto())
        {
            if (!_inited)
                return;

            if (!entities.IsCreated || entities.Length != _enemyQuery.CalculateEntityCount())
            {
                if (entities.IsCreated) entities.Dispose();
                entities = _enemyQuery.ToEntityArray(Allocator.Persistent);
                _renderData.Clear();
                _entityManager.GetAllUniqueSharedComponentsManaged(_renderData);

            }


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

            if (_renderData.Count <= 1)
                return;


            a = _enemyQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
            b = _enemyQuery.ToComponentDataArray<MaterialOverrideOffset>(Allocator.Temp);


            Render();
            context.ExecuteCommandBuffer(_commandBuffer);

        }
    }

    public void Render()
    {
        using (RenderMarker.Auto()) // Profiling Render method
        {
            if (a.Count() == 0 || b.Count() == 0)
                return;

            data = _renderData[1];

            // foreach (var entity in entities)
            // {
            //     Batch(entity);
            // }

            int count = entities.Count();
            batchCount = count;
            for (int i = 0; i < count; i++)
            {
                var loc = a[i];
                matrix[i] = loc.Value;

                var materialOverride = b[i];
                offsetScaleArray[i] = new Vector4(materialOverride.Offset.x, materialOverride.Offset.y,
                                                           materialOverride.Scale.x, materialOverride.Scale.y);

                var spriteFrameBuffer = _entityManager.GetBuffer<SpriteFrameElement>(entities[i]);
                if (spriteFrameBuffer.Length > 0)
                {
                    spriteOffsetArray[i] = spriteFrameBuffer[0].offset;
                }
                else
                {
                    spriteOffsetArray[i] = Vector2.zero;
                }
            }

            Submit();


            a.Dispose();
            b.Dispose();
        }
    }

    private void Submit()
    {
        using (SubmitMarker.Auto()) // Profiling Submit method
        {
            if (batchCount == 0)
                return;

            if (data.Mesh == null || data.Material == null)
                return;

            SetMaterialProperties();

            _commandBuffer.DrawMeshInstanced(data.Mesh, data.SubMesh, data.Material, 0, matrix, batchCount, propertyBlock);
            batchCount = 0;
        }
    }

    private readonly List<Vector4> spriteOffsetList = new List<Vector4>(batchSize);

    private void SetMaterialProperties()
    {
        //  offsetScaleList.Clear();
        spriteOffsetList.Clear();

        //  offsetScaleList.AddRange(offsetScaleArray);
        for (int i = 0; i < batchCount; i++)
        {
            spriteOffsetList.Add(new Vector4(spriteOffsetArray[i].x, spriteOffsetArray[i].y, 0, 0));
        }

        propertyBlock.SetVectorArray("_OffsetXYScaleZW", offsetScaleArray);
        propertyBlock.SetVectorArray("_SpriteOffset", spriteOffsetList);
        propertyBlock.SetTexture("_MainTex", ConfigManager.Instance.textureSheet);
    }
}
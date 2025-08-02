using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlueTintRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class BlueTintSettings
    {
        public Material tintMaterial;
    }

    public BlueTintSettings settings = new BlueTintSettings();

    class BlueTintPass : ScriptableRenderPass
    {
        private Material tintMaterial;
        private RTHandle cameraColorTarget;
        private RTHandle temporaryColorTexture;
        private string profilerTag;
        public bool buildMode = false;

        public BlueTintPass(Material material, string tag)
        {
            tintMaterial = material;
            profilerTag = tag;
        }

        public void Setup(RTHandle colorTarget)
        {
            cameraColorTarget = colorTarget;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderingUtils.ReAllocateIfNeeded(ref temporaryColorTexture, cameraTextureDescriptor, name: "_BlueTintTempTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!buildMode || tintMaterial == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            // Copy screen to temp
            Blitter.BlitCameraTexture(cmd, cameraColorTarget, temporaryColorTexture);

            // Blit temp + tint back to screen
            Blitter.BlitCameraTexture(cmd, temporaryColorTexture, cameraColorTarget, tintMaterial, 0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            temporaryColorTexture?.Release();
        }
    }

    private BlueTintPass blueTintPass;
    public static BlueTintRenderFeature Instance;

    public override void Create()
    {
        Instance = this;
        blueTintPass = new BlueTintPass(settings.tintMaterial, "Blue Tint URP Pass");
        blueTintPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.tintMaterial == null)
            return;

        blueTintPass.Setup(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(blueTintPass);
    }

    public void SetBuildMode(bool enable)
    {
        if (blueTintPass != null)
        {
            blueTintPass.buildMode = enable;
        }
    }
}

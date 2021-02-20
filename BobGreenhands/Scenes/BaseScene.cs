using System.Text;
using Nez;
using Nez.UI;
using BobGreenhands.Utils;
using System;


namespace BobGreenhands.Scenes
{
    /// <summary>
    /// the Scene every Scene in this game bases on. Has some preset properties like the Background Color and initializes the UI Canvas
    /// </summary>
    public abstract class BaseScene : Scene
    {
        public UICanvas UICanvas
        {
            get;
            protected set;
        }

        public Label DebugLabel = new Label("Loading...\n\n\n\n\n\n\n\n\n\n\n", Game.NormalSkin.Get<LabelStyle>("debugLabel"));

        public static readonly int UIRenderLayer = 100;

        // make sure that the debug renderer is rendering above everything else
        public static readonly int DebugRenderLayer = Int32.MinValue;

        protected ScreenSpaceRenderer ScreenSpaceRenderer;

        public BaseScene() : base()
        {
            ScreenSpaceRenderer = new ScreenSpaceRenderer(10, UIRenderLayer);
            ScreenSpaceRenderer debugRenderer = new ScreenSpaceRenderer(Int32.MaxValue, DebugRenderLayer);
            debugRenderer.WantsToRenderAfterPostProcessors = true;
            AddRenderer(ScreenSpaceRenderer);
            AddRenderer(debugRenderer);
            ClearColor = Game.NormalSkin.BackgroundColor;
            UICanvas = CreateEntity("ui-canvas").AddComponent(new UICanvas());
            UICanvas.RenderLayer = UIRenderLayer;
        }

        ~BaseScene()
        {
            // in case the scene has implemented the input processor, unsubscribe it from the handler
            IInputProcessor? inputProc = this as IInputProcessor;
            if (inputProc != null && Game.IsSubscribedToInputHandler(inputProc))
            {
                Game.UnsubscribeFromInputHandler(inputProc);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            UICanvas DebugCanvas = CreateEntity("debug-canvas").AddComponent(new UICanvas());
            DebugCanvas.RenderLayer = DebugRenderLayer;
            SetDesignResolution(640, 360, SceneResolutionPolicy.FixedHeight, 0, 0);
            Table debugTable = DebugCanvas.Stage.AddElement(new Table());
            debugTable.SetFillParent(true).Top();
            //debugTable.SetDebug(true);
            DebugLabel.SetFontScale(1f);
            debugTable.Add(DebugLabel).SetExpandX().SetFillX().Left().Top();
        }

        public override void Update()
        {
            base.Update();
            if (Game.DebugRenderEnabled)
            {
                double fps = Math.Round(1 / Time.DeltaTime, 0);
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("FPS: {0}\n", fps);
                builder.AppendFormat("Delta: {0:0.000000}ms\n", Time.DeltaTime);
                builder.AppendFormat("Scene: {0}\n", Game.Scene.ToString());
                builder.AppendFormat("Resolution: {0}x{1}\n", Screen.Width, Screen.Height);
                builder.AppendFormat("OS: {0}\n", Program.OSInformation);
                builder.AppendFormat("CPU: {0}\n", Program.CPUInformation);
                builder.AppendFormat("GPU: {0}\n", Program.GPUInformation);
                builder.AppendFormat("Mem: {0}\n", Program.MemInformation);
                builder.AppendFormat("Entities: {0} | CamPos: [{1:0.00}|{2:0.00}]\n", Entities.Count, Camera.Position.X, Camera.Position.Y);
                DebugLabel.SetVisible(true);
                DebugLabel.SetText(builder.ToString());
            } else {
                DebugLabel.SetVisible(false);
            }
        }
    }
}
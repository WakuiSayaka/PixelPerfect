using System;
using Dalamud.Plugin;
using ImGuiNET;
using ImGuiScene;
using Dalamud.Configuration;
using Num = System.Numerics;
using Dalamud.Game.Command;
using Dalamud.Interface;

namespace PixelPerfectPlus
{
    public class PixelPerfectPlus : IDalamudPlugin
    {
        public string Name => "Pixel Perfect Plus";
        private DalamudPluginInterface _pluginInterface;
        private Config _configuration;
        private bool _enabled = true;
        private bool _config;
        private bool _combat = true;
        private bool _circle;
        private bool _instance;
        private Num.Vector4 _col = new Num.Vector4(1f, 1f, 1f, 1f);
        private Num.Vector4 _col2 = new Num.Vector4(0.4f, 0.4f, 0.4f, 1f);
        private bool _ring;
        private Num.Vector4 _colRing = new Num.Vector4(0.4f, 0.4f, 0.4f, 0.5f);
        private float _radius = 10f;
        private int _segments = 100;
        private float _thickness = 10f;
        //2
        private bool _ring2;
        private Num.Vector4 _colRing2 = new Num.Vector4(0.4f, 0.4f, 0.4f, 0.5f);
        private float _radius2 = 10f;
        private int _segments2 = 100;
        private float _thickness2 = 10f;
        //3
        private bool _ring3;
        private Num.Vector4 _colRing3 = new Num.Vector4(0.4f, 0.4f, 0.4f, 0.5f);
        private float _radius3 = 10f;
        private int _segments3 = 100;
        private float _thickness3 = 10f;

        public void Initialize(DalamudPluginInterface pI)
        {
            _pluginInterface = pI;
            _configuration = _pluginInterface.GetPluginConfig() as Config ?? new Config();
            _ring = _configuration.Ring;
            _thickness = _configuration.Thickness;
            _colRing = _configuration.ColRing;
            _segments = _configuration.Segments;
            _radius = _configuration.Radius;
            //2
            _ring2 = _configuration.Ring2;
            _thickness2 = _configuration.Thickness2;
            _colRing2 = _configuration.ColRing2;
            _segments2 = _configuration.Segments2;
            _radius2 = _configuration.Radius2;

            //3
            _ring3 = _configuration.Ring3;
            _thickness3 = _configuration.Thickness3;
            _colRing3 = _configuration.ColRing3;
            _segments3 = _configuration.Segments3;
            _radius3 = _configuration.Radius3;

            _enabled = _configuration.Enabled;
            _combat = _configuration.Combat;
            _circle = _configuration.Circle;
            _instance = _configuration.Instance;
            _col = _configuration.Col;
            _col2 = _configuration.Col2;
            _pluginInterface.UiBuilder.OnBuildUi += DrawWindow;
            _pluginInterface.UiBuilder.OnOpenConfigUi += ConfigWindow;
            _pluginInterface.CommandManager.AddHandler("/ppp", new CommandInfo(Command)
            {
                HelpMessage = "Pixel Perfect Plus config."
            });
        }

        public void Dispose()
        {
            _pluginInterface.UiBuilder.OnBuildUi -= DrawWindow;
            _pluginInterface.UiBuilder.OnOpenConfigUi -= ConfigWindow;
            _pluginInterface.CommandManager.RemoveHandler("/ppp");
        }

        private void DrawWindow()
        {
            if (_config)
            {
                ImGui.SetNextWindowSize(new Num.Vector2(300, 500), ImGuiCond.FirstUseEver);
                ImGui.Begin("Pixel Perfect Plus Config", ref _config);
                ImGui.Checkbox("Hitbox", ref _enabled);
                ImGui.Checkbox("Outer Ring", ref _circle);
                ImGui.Checkbox("Combat Only", ref _combat);
                ImGui.Checkbox("Instance Only", ref _instance);
                ImGui.ColorEdit4("Colour", ref _col, ImGuiColorEditFlags.NoInputs);
                ImGui.ColorEdit4("Outer Colour", ref _col2, ImGuiColorEditFlags.NoInputs);
                ImGui.Separator();
                ImGui.Checkbox("Ring", ref _ring);
                ImGui.DragFloat("Yalms", ref _radius);
                ImGui.DragFloat("Thickness", ref _thickness);
                ImGui.DragInt("Smoothness", ref _segments);
                ImGui.ColorEdit4("Ring Colour", ref _colRing, ImGuiColorEditFlags.NoInputs);
                ImGui.Separator(); //2
                ImGui.Checkbox("Ring2", ref _ring2);
                ImGui.DragFloat("Ring2_Yalms", ref _radius2);
                ImGui.DragFloat("Ring2_Thickness", ref _thickness2);
                ImGui.DragInt("Ring2_Smoothness", ref _segments2);
                ImGui.ColorEdit4("Ring2_Ring Colour", ref _colRing2, ImGuiColorEditFlags.NoInputs);
                ImGui.Separator(); //3
                ImGui.Checkbox("Ring3", ref _ring3);
                ImGui.DragFloat("Ring3_Yalms", ref _radius3);
                ImGui.DragFloat("Ring3_Thickness", ref _thickness3);
                ImGui.DragInt("Ring3_Smoothness", ref _segments3);
                ImGui.ColorEdit4("Ring3_Ring Colour", ref _colRing3, ImGuiColorEditFlags.NoInputs);

                if (ImGui.Button("Save and Close Config"))
                {
                    SaveConfig();
                    _config = false;
                }
                ImGui.SameLine();
                //ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
                //ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFF);
                //ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);

                if (ImGui.Button("Profiles"))
                {
                    Profiles();
                }
                //ImGui.PopStyleColor(3);
                ImGui.End();
            }

            //hitboxチェック
            if (!_enabled || _pluginInterface.ClientState.LocalPlayer == null) return;
            if(_combat)
            {
                if (!_pluginInterface.ClientState.Condition[Dalamud.Game.ClientState.ConditionFlag.InCombat]) { return; }
            }

            if (_instance)
            {
                if (!_pluginInterface.ClientState.Condition[Dalamud.Game.ClientState.ConditionFlag.BoundByDuty]) { return; }
            }

            var actor = _pluginInterface.ClientState.LocalPlayer;
            if (!_pluginInterface.Framework.Gui.WorldToScreen(
                new SharpDX.Vector3(actor.Position.X, actor.Position.Z, actor.Position.Y),
                out var pos)) return;
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Num.Vector2(pos.X - 10 - ImGuiHelpers.MainViewport.Pos.X, pos.Y - 10- ImGuiHelpers.MainViewport.Pos.Y));
            ImGui.Begin("Pixel Perfect Plus", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoBackground);
            ImGui.GetWindowDrawList().AddCircleFilled(
                new Num.Vector2(pos.X, pos.Y),
                2f,
                ImGui.GetColorU32(_col),
                100);


            //15m後方にcircle キャラクターの角度の参考: https://goatcorp.github.io/Dalamud/api/Dalamud.Game.ClientState.Actors.Types.Actor.html#Dalamud_Game_ClientState_Actors_Types_Actor_Rotation　
            //角度と長さをxy座標に変換した後2次元座標に変換して描画
            //WorldToScreenでゲーム内の3次元座標を2次元座標に変換->ImGuiで2次元座標を指定してモニターに描画 （DrawRingWorld）



            if (_circle)
            {
                ImGui.GetWindowDrawList().AddCircle(
                    new Num.Vector2(pos.X, pos.Y),
                    2.2f,
                    ImGui.GetColorU32(_col2),
                    100);
            }
            ImGui.End();

            if (!_ring && !_ring2 && !_ring3) return;

            if (_ring) { 
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Num.Vector2(0, 0));
                ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Num.Vector2(0, 0));
                ImGui.Begin("Ring", ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);
                ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
                DrawRingWorld(_pluginInterface.ClientState.LocalPlayer, _radius, _segments, _thickness, ImGui.GetColorU32(_colRing));
                ImGui.End();
                ImGui.PopStyleVar();
            }

            //2
            if (_ring2) {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Num.Vector2(0, 0));
                ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Num.Vector2(0, 0));
                ImGui.Begin("Ring2", ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);
                ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
                DrawRingWorld(_pluginInterface.ClientState.LocalPlayer, _radius2, _segments2, _thickness2, ImGui.GetColorU32(_colRing2));
                ImGui.End();
                ImGui.PopStyleVar();
            }


            //3
            if (_ring3)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Num.Vector2(0, 0));
                ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Num.Vector2(0, 0));
                ImGui.Begin("Ring3", ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);
                ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
                DrawRingWorld(_pluginInterface.ClientState.LocalPlayer, _radius3, _segments3, _thickness3, ImGui.GetColorU32(_colRing3));
                ImGui.End();
                ImGui.PopStyleVar();
            }


        }
        private void ConfigWindow(object sender, EventArgs args)
        {
            _config = true;
        }

        private void Command(string command, string arguments)
        {
            _config = true;
        }

        private void SaveConfig()
        {
            _configuration.Enabled = _enabled;
            _configuration.Combat = _combat;
            _configuration.Circle = _circle;
            _configuration.Instance = _instance;
            _configuration.Col = _col;
            _configuration.Col2 = _col2;
            _configuration.ColRing = _colRing;
            _configuration.Thickness = _thickness;
            _configuration.Segments = _segments;
            _configuration.Ring = _ring;
            _configuration.Radius = _radius;
            //2
            _configuration.ColRing2 = _colRing2;
            _configuration.Thickness2 = _thickness2;
            _configuration.Segments2 = _segments2;
            _configuration.Ring2 = _ring2;
            _configuration.Radius2 = _radius2;
            //3
            _configuration.ColRing3 = _colRing3;
            _configuration.Thickness3 = _thickness3;
            _configuration.Segments3 = _segments3;
            _configuration.Ring3 = _ring3;
            _configuration.Radius3 = _radius3;
            _pluginInterface.SavePluginConfig(_configuration);
        }


        private void Profiles()
        {
            //ジョブプロファイル画面
        }

        private void DrawRingWorld(Dalamud.Game.ClientState.Actors.Types.Actor actor, float radius, int numSegments, float thicc, uint colour)
        {
            var seg = numSegments / 2;
            for (var i = 0; i <= numSegments; i++)
            {
                //WorldToScreen(x,y,z,pos) ゲーム内の３次元座標（x,y,z）を指定して2次元座標に変換されたものがpos(pox.X,pos.Y)に入る？
                _pluginInterface.Framework.Gui.WorldToScreen(new SharpDX.Vector3(actor.Position.X + (radius * (float)Math.Sin((Math.PI / seg) * i)), actor.Position.Z, actor.Position.Y + (radius * (float)Math.Cos((Math.PI / seg) * i))), out SharpDX.Vector2 pos);
                ImGui.GetWindowDrawList().PathLineTo(new Num.Vector2(pos.X, pos.Y));
            }
            ImGui.GetWindowDrawList().PathStroke(colour, ImDrawFlags.Closed, thicc);
        }
    }

    public class Config : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool Enabled { get; set; } = true;
        public bool Combat { get; set; } = true;
        public bool Circle { get; set; }
        public bool Instance { get; set; }
        public Num.Vector4 Col { get; set; } = new Num.Vector4(1f, 1f, 1f, 1f);
        public Num.Vector4 Col2 { get; set; } = new Num.Vector4(0.4f, 0.4f, 0.4f, 1f);
        public Num.Vector4 ColRing { get; set; } = new Num.Vector4(0.4f, 0.4f, 0.4f, 0.5f);
        public int Segments { get; set; } = 100;
        public float Thickness { get; set; } = 10f;
        public bool Ring { get; set; }
        public float Radius { get; set; } = 2f;
        public Num.Vector4 ColRing2 { get; set; } = new Num.Vector4(0.4f, 0.4f, 0.4f, 0.5f);
        public int Segments2 { get; set; } = 100;
        public float Thickness2 { get; set; } = 10f;
        public bool Ring2 { get; set; }
        public float Radius2 { get; set; } = 2f;
        public Num.Vector4 ColRing3 { get; set; } = new Num.Vector4(0.4f, 0.4f, 0.4f, 0.5f);
        public int Segments3 { get; set; } = 100;
        public float Thickness3 { get; set; } = 10f;
        public bool Ring3 { get; set; }
        public float Radius3 { get; set; } = 2f;


    }

}

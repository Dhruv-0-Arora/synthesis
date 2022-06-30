using Synthesis.UI.Dynamic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseModeModal : ModalDynamic
{
    public ChooseModeModal() : base(new Vector2(300, 120)) {}

    public override void Create()
    {
        Title.SetText("Choose Mode");
        Description.SetText("Choose a mode to play in.");

        MainContent.CreateButton()
            .StepIntoLabel(l => l.SetText("Practice Mode"))
            .ApplyTemplate(Button.VerticalLayoutTemplate)
            .AddOnClickedEvent(b =>
            {
                ModeManager.CurrentMode = ModeManager.Mode.Practice;
                SceneManager.LoadScene("MainScene");
            });

        MainContent.CreateButton()
            .StepIntoLabel(l => l.SetText("Match Mode"))
            .ApplyTemplate(Button.VerticalLayoutTemplate)
            .AddOnClickedEvent(b =>
            {
                ModeManager.CurrentMode = ModeManager.Mode.Match;
                SceneManager.LoadScene("MainScene");
            });
    }
    
    public override void Update() {}

    public override void Delete()
    {
    }
}
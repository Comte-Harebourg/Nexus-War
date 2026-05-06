using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    public GameSettings settings;
    public GameObject MainMenu, PlayMenu, OptionMenu;
    public TextMeshProUGUI ValueAnimationSpeed, ValuePopUpSize, ValueLevel;
    public Slider SliderAnimationSpeed, SliderPopUpSize, SliderLevel;
    public Toggle ToggleSkipAnimation, ToggleBot, ToggleAberrion, ToggleOromound, ToggleSeranna, ToggleAutoEndTurn;
    public bool SkipAnimation, Bot, AutoEndTurn;
    public float AnimationSpeed;
    public int PopUpSize, Level, Faction;

    void Awake()
    {
        Instance = this;
        ToggleSkipAnimation.isOn = settings.SkipAnimation;
        ToggleBot.isOn = settings.Bot;
        ToggleAutoEndTurn.isOn = settings.AutoEndTurn;
        SliderAnimationSpeed.value = settings.AnimationSpeed;
        SliderPopUpSize.value = settings.PopUpSize;
        UpdateSkipAnimation();
        UpdateAnimationSpeed();
        UpdatePopUpSize();
        SliderLevel.maxValue = Resources.LoadAll<ScriptableLevel>("Levels").Length - 1;
        ToggleAberrion.isOn = true;
    }

    public void ShowPlay()
    {
        MainMenu.SetActive(false);
        PlayMenu.SetActive(true);
        OptionMenu.SetActive(false);
    }

    public void ShowOption()
    {
        MainMenu.SetActive(false);
        PlayMenu.SetActive(false);
        OptionMenu.SetActive(true);
    }

    public void Cancel()
    {
        MainMenu.SetActive(true);
        PlayMenu.SetActive(false);
        OptionMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateSkipAnimation()
    {
        SkipAnimation = ToggleSkipAnimation.isOn;
        SliderAnimationSpeed.interactable = !SkipAnimation;
    }

    public void UpdateBot()
    {
        Bot = ToggleBot.isOn;
    }

    public void UpdateAutoEndTurn()
    {
        AutoEndTurn = ToggleAutoEndTurn.isOn;
    }

    public void UpdateAnimationSpeed()
    {
        AnimationSpeed = Mathf.Round(SliderAnimationSpeed.value * 100.0f) * 0.01f;
        ValueAnimationSpeed.text = Mathf.Round(SliderAnimationSpeed.value * 100.0f).ToString();
    }

    public void UpdatePopUpSize()
    {
        PopUpSize = (int)SliderPopUpSize.value;
        ValuePopUpSize.text = (SliderPopUpSize.value).ToString();
    }

    public void Reset()
    {
        ToggleSkipAnimation.isOn = false;
        ToggleBot.isOn = true;
        ToggleAutoEndTurn.isOn = false;
        SliderAnimationSpeed.value = 0.15f;
        SliderPopUpSize.value = 10;
        UpdateAnimationSpeed();
        UpdatePopUpSize();
    }

    public void StartLevel()
    {
        settings.SkipAnimation = SkipAnimation;
        settings.Bot = Bot;
        settings.AnimationSpeed = AnimationSpeed;
        settings.PopUpSize = PopUpSize;
        settings.AutoEndTurn = AutoEndTurn;
        settings.Level = Level;
        settings.Faction = Faction;
        SceneManager.LoadSceneAsync("CombatMap");
    }

    public void UpdateLevel()
    {
        Level = (int)SliderLevel.value;
        ValueLevel.text = (Level+1).ToString();
    }

    public void UpdateAberrion()
    {
        if (Faction == 0) ToggleAberrion.isOn = true;
        else
        {
            Faction = 0;
            ToggleOromound.isOn = false;
            ToggleSeranna.isOn = false;
        }
    }

    public void UpdateOromound()
    {
        if (Faction == 1) ToggleOromound.isOn = true;
        else
        {
            Faction = 1;
            ToggleAberrion.isOn = false;
            ToggleSeranna.isOn = false;
        }
    }

    public void UpdateSeranna()
    {
        if (Faction == 2) ToggleSeranna.isOn = true;
        else
        {
            Faction = 2;
            ToggleAberrion.isOn = false;
            ToggleOromound.isOn = false;
        }
    }
}
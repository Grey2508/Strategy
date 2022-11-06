using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] Image Background;
    [SerializeField] float BackgroundDurationVisible = 1;

    [SerializeField] Text GameoverText;
    [SerializeField] float GameoverDurationIncrease = 1;

    [SerializeField] Text GameSessionTime;
    [SerializeField] float GameSessionTimeIncrease = 1;

    [SerializeField] GameObject Buttons;
    [SerializeField] float ButtonsDurationVisible = 1;

    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
    }

    public void StartAnimation()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        _canvas.enabled = true;

        for (float t = 0; t < 1; t += Time.deltaTime / BackgroundDurationVisible)
        {
            Color color = Background.color;
            color.a = t;
            Background.color = color;

            yield return null;
        }

        for(float t = 0; t<1; t+=Time.deltaTime/GameoverDurationIncrease)
        {
            GameoverText.rectTransform.localScale = Vector2.one * t;

            yield return null;
        }

        GameSessionTime.text = GetTime();

        for (float t = 0; t < 1; t += Time.deltaTime / GameSessionTimeIncrease)
        {
            GameSessionTime.rectTransform.localScale = Vector2.one * t;

            yield return null;
        }

        for (float t = 0; t < 1; t += Time.deltaTime / ButtonsDurationVisible)
        {
            Buttons.transform.localScale = Vector2.one * t;

            yield return null;
        }
    }

    private string GetTime()
    {
        int time = (int)Time.timeSinceLevelLoad;

        int min = time / 60;
        int sec = time % 60;

        string minStr = min < 10 ? $"0{min}" : min.ToString();
        string secStr = sec < 10 ? $"0{sec}" : sec.ToString();

        return $"{minStr}:{secStr}";
    }

}

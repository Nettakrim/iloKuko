using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private TokiInterpreter tokiInterpreter;
    [SerializeField] private Cyberspace cyberspace;

    private Stage stage;

    [SerializeField] private GameObject[] stages;
    [SerializeField] private GameObject wrongBox;

    [SerializeField] private SitelenPona pokiInfo;
    [SerializeField] private string[] boxNames;

    void OnEnable()
    {
        cyberspace.LockInput(true);
    }

    void LateUpdate()
    {
        int current = cyberspace.GetCurrentBox();
        Cyberspace.State state = cyberspace.GetState();

        if (stage >= Stage.Poki)
        {
            pokiInfo.SetMessage("poki ni li " + boxNames[current], "");
        }

        if (stage == Stage.Poki && state == Cyberspace.State.Box && current == 0)
        {
            Advance();
        }

        wrongBox.SetActive(false);

        if (stage >= Stage.Poki && (current != 0 || state != Cyberspace.State.Box))
        {
            for (int i = 0; i < stages.Length; i++)
            {
                stages[i].SetActive(false);
            }

            if (state == Cyberspace.State.Cyberspace)
            {
                stages[1].SetActive(true);
            }
            else if (state == Cyberspace.State.Box)
            {
                wrongBox.SetActive(true);
            }
            return;
        }

        for (int i = 0; i < stages.Length; i++)
        {
            stages[i].SetActive(i == (int)stage);
        }
    }

    public void Advance()
    {
        if (stage == Stage.Pini)
        {
            SceneManager.LoadScene(0);
            return;
        }

        stage += 1;

        if (stage == Stage.Poki)
        {
            cyberspace.LockInput(false);
        }

        if (stage == Stage.Kute)
        {
            tokiInterpreter.CallFunction("#1", true);
        }

        if (stage == Stage.Soweli)
        {
            tokiInterpreter.CallFunction("#2", true);
        }
    }

    enum Stage
    {
        Alasa,
        Poki,
        Kute,
        Soweli,
        Pana,
        Weka,
        Pini
    }
}

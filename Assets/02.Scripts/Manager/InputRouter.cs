using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRouter : Singleton<InputRouter>
{
    public bool SpacePressed { get; private set; }
    public bool EscapePressed { get; private set; }
    public bool TabPressed { get; private set; }
    public bool PressedE { get; private set; }
    public bool PressedR { get; private set; }
    public bool PressedQ { get; private set; }
    public bool PressedW { get; private set; }
    public bool PressedA { get; private set; }
    public bool PressedS { get; private set; }
    public bool PressedD { get; private set; }

    private bool spaceConsumed;
    private bool escapeConsumed;
    private bool tabConsumed;
    private bool eConsumed;
    private bool rConsumed;
    private bool qConsumed;
    private bool wConsumed;
    private bool aConsumed;
    private bool sConsumed;
    private bool dConsumed;

    void Update()
    {
        SpacePressed = Input.GetKeyDown(KeyCode.Space);
        spaceConsumed = false;

        EscapePressed = Input.GetKeyDown(KeyCode.Escape);
        escapeConsumed = false;

        TabPressed = Input.GetKeyDown(KeyCode.Tab);
        tabConsumed = false;

        PressedE = Input.GetKeyDown(KeyCode.E);
        eConsumed = false;

        PressedR = Input.GetKeyDown(KeyCode.R);
        rConsumed = false;

        PressedQ = Input.GetKeyDown(KeyCode.Q);
        qConsumed = false;

        PressedW = Input.GetKeyDown(KeyCode.W);
        wConsumed = false;

        PressedA = Input.GetKeyDown(KeyCode.A);
        aConsumed = false;

        PressedS = Input.GetKeyDown(KeyCode.S);
        sConsumed = false;

        PressedD = Input.GetKeyDown(KeyCode.D);
        dConsumed = false;
    }

    public bool ConsumeSpace()
    {
        if (SpacePressed && !spaceConsumed)
        {
            spaceConsumed = true;
            return true;
        }
        return false;
    }

    public bool ConsumeEscape()
    {
        if (EscapePressed && !escapeConsumed)
        {
            escapeConsumed = true;
            return true;
        }
        return false;
    }

    public bool ConsumeTab()
    {
        if (TabPressed && !tabConsumed)
        {
            tabConsumed = true;
            return true;
        }
        return false;
    }

    public bool ConsumeE()
    {
        if (PressedE && !eConsumed)
        {
            eConsumed = true;
            return true;
        }
        return false;
    }

    public bool ConsumeR()
    {
        if (PressedR && !rConsumed)
        {
            rConsumed = true;
            return true;
        }
        return false;
    }

    public bool ConsumeQ()
    {
        if (PressedQ && !qConsumed)
        {
            qConsumed = true;
            return true;
        }
        return false;
    }

    public bool ConsumeW()
    {
        if (PressedW && !wConsumed)
        {
            wConsumed = true;
            return true;
        }
        return false;
    }

    public bool ConsumeA()
    {
        if (PressedA && !aConsumed)
        {
            aConsumed = true;
            return true;
        }
        return false;
    }

    public bool ConsumeS()
    {
        if (PressedS && !sConsumed)
        {
            sConsumed = true;
            return true;
        }
        return false;
    }

    public bool ConsumeD()
    {
        if (PressedD && !dConsumed)
        {
            dConsumed = true;
            return true;
        }
        return false;
    }
}

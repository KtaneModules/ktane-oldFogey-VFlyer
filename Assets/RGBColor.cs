using System;

class RGBColor
{
    public bool r;
    public bool g;
    public bool b;

    public RGBColor()
    {
        r = false;
        g = false;
        b = false;
    }

    public RGBColor(bool r, bool g, bool b)
    {
        this.r = r;
        this.b = b;
        this.g = g;
    }

    public RGBColor(int color)
    {
        r = (color / 100) != 0;
        g = ((color % 100) / 10) != 0;
        b = (color % 10) != 0;
    }

    public string GetName()
    {
        switch((r ? 100 : 0) + (g ? 10 : 0) + (b ? 1 : 0))
        {
            case 0: return "None";
            case 1: return "Blue";
            case 10: return "Green";
            case 100: return "Red";
            case 11: return "Cyan";
            case 101: return "Magenta";
            case 110: return "Yellow";
            case 111: return "White";
        }

        return "N/A";
    }

    public bool Equals(RGBColor color)
    {
        return r == color.r && g == color.g && b == color.b;
    }

    public RGBColor Sum(RGBColor color)
    {
        return new RGBColor(r ^ color.r, g ^ color.g, b ^ color.b);
    }

    public RGBColor Clone()
    {
        return new RGBColor(r, g, b);
    }

    public int GetTapeNumber()
    {
        if(r && !g && !b)
            return 0;

        if(!r && g && !b)
            return 1;

        if(!r && !g && b)
            return 2;

        if((!r && !g && !b) || (r && g && b))
            return 4;

        return 3;
    }
}
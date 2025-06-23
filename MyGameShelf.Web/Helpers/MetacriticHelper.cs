namespace MyGameShelf.Web.Helpers;

public static class MetacriticHelper
{
    public static string GetMetacriticColor(int score)
    {
        if (score >= 95) return "#4caf50";      // Perfect
        if (score >= 85) return "#8bc34a";      // Great
        if (score >= 75) return "#cddc39";      // Good
        if (score >= 60) return "#ffeb3b";      // Fair
        if (score >= 45) return "#ff9800";      // Poor
        if (score >= 30) return "#f44336";      // Very Poor
        return "#b71c1c";                       // Unacceptable
    }

    public static string GetMetacriticBorder(int score)
    {
        if (score >= 95) return "#388e3c";
        if (score >= 85) return "#689f38";
        if (score >= 75) return "#afb42b";
        if (score >= 60) return "#fbc02d";
        if (score >= 45) return "#ef6c00";
        if (score >= 30) return "#c62828";
        return "#7f0000";
    }
}
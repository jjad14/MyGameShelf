namespace MyGameShelf.Web.Helpers;

public static class QrCodeHelper
{
    public static string FormatKey(string unformattedKey)
    {
        // Format key 4 character spaced
        return string.Join(" ", Enumerable.Range(0, unformattedKey.Length / 4)
            .Select(i => unformattedKey.Substring(i * 4, 4)));
    }

    public static string GenerateQrCodeUri(string email, string unformattedKey)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(unformattedKey))
            return string.Empty;

        // Return generated QR Code
        return string.Format(
            "otpauth://totp/{0}?secret={1}&issuer={2}&digits=6",
            Uri.EscapeDataString("MyGameShelf:" + email),
            unformattedKey,
            Uri.EscapeDataString("MyGameShelf"));
    }
}

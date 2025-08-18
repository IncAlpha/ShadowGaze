using Net.Codecrete.QrCodeGenerator;

namespace ShadowGaze.Core.Services.QrCodes;

public static class QrCodeGenerator
{
    public static byte[] GetQrImage(string content)
    {
        var qrCode = QrCode.EncodeText(content, QrCode.Ecc.Medium);
        return qrCode.ToBmpBitmap(2, 10);
    }
}
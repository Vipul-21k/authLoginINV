using authLogin.Data;
using authLogin.Models;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using QRCoder;

namespace authLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Signup Page
        public IActionResult Signup()
        {
            return View();
        }

        // Login Page
        public IActionResult Login()
        {
            return View();
        }

        // Generate Secret Key
        private string GenerateSecretKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        // Generate QR Code
        private string GenerateQrCode(string email, string secretKey)
        {
            var issuer = "AuthDemoApp";

            var otpUri = $"otpauth://totp/{issuer}:{email}?secret={secretKey}&issuer={issuer}";

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new Base64QRCode(qrCodeData);

                return qrCode.GetGraphic(5);
            }
        }

        // Signup POST
        [HttpPost]
        public IActionResult Signup(ApplicationUser user)
        {
            if (user.Is2FAEnabled)
            {
                var secretKey = GenerateSecretKey();
                user.TwoFactorSecretKey = secretKey;

                user.Is2FAEnabled = false;
                user.Is2FASetupCompleted = false;

                _context.Users.Add(user);
                _context.SaveChanges();

                var qrCode = GenerateQrCode(user.Email, secretKey);

                TempData["UserEmail"] = user.Email;
                ViewBag.QRCode = qrCode;

                return View("Setup2FA");
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // Login POST
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                return Content("User not found");
            }

            if (user.PasswordHash != password)
            {
                return Content("Invalid password");
            }

            if (user.Is2FAEnabled && user.Is2FASetupCompleted)
            {
                TempData["UserEmail"] = user.Email;
                return RedirectToAction("VerifyOTP");
            }

            return RedirectToAction("Dashboard");
        }

        // OTP Page
        public IActionResult VerifyOTP()
        {
            return View();
        }

        // OTP Verify
        [HttpPost]
        public IActionResult VerifyOTP(string otp)
        {
            var email = TempData["UserEmail"]?.ToString();

            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var secretKey = Base32Encoding.ToBytes(user.TwoFactorSecretKey);

            var totp = new Totp(secretKey);

            bool isValid = totp.VerifyTotp(
                otp,
                out long timeStepMatched,
                VerificationWindow.RfcSpecifiedNetworkDelay
            );

            if (isValid)
            {
                return RedirectToAction("Dashboard");
            }

            return Content("Invalid OTP");
        }

        /// After QR Scan verify OTP 
[HttpPost]
public IActionResult VerifySetupOTP(string otp)
{
    var email = TempData["UserEmail"]?.ToString();

    var user = _context.Users.FirstOrDefault(x => x.Email == email);

    var secretKey = Base32Encoding.ToBytes(user.TwoFactorSecretKey);

    var totp = new Totp(secretKey);

    bool isValid = totp.VerifyTotp(otp, out long step);

    if (isValid)
    {
        user.Is2FAEnabled = true;
        user.Is2FASetupCompleted = true;

        _context.SaveChanges();

        return RedirectToAction("Login");
    }

    return Content("Invalid OTP");
}
        // redirect dashboard after login 
        public IActionResult Dashboard()
        {
            return View();
        }

        //after logout redirect to login page 
        public IActionResult Logout()
        {
            TempData.Clear();

            return RedirectToAction("Login");
        }
    }
}
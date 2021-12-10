using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TokenExampleProject.Controllers
{
    public class HomeController : Controller
    {
        private TokenType token { get; set; }
        public IToken Provider { get; }
        private ITokenData Data { get; }
        public ActionResult Index()
        {
            return View();
        }

        private void Login()
        {
            if (isLogin == false)
            {
                var ILogin = (Provider as ILogin);
                if (ILogin.Login())
                {
                    token.TokenId = ILogin.TokenId();
                    token.Token = ILogin.Token();
                    token.CreationTime = DateTime.Now;
                    token.Id = Data.InsertToken(token);
                }
            }
        }

        public bool isLogin
        {
            get
            {
                bool r = false;
                if (Provider is ILogin)
                {
                    var ILogin = (Provider as ILogin);
                    if (token == null)
                    {
                        token = Data.GetToken(ILogin.TokenId());
                        ILogin.LoadToken(token.Token);
                    }

                    if (TokenSonKullanmaKontrol(ILogin.ExpirationTimeMinute()))
                    {
                        r = true;
                    }

                }
                else
                {
                    r = true;
                }

                return r;
            }
        }

        private bool TokenSonKullanmaKontrol(int dakika)
        {
            return token.CreationTime.AddMinutes(dakika) > DateTime.Now;
        }
    }

    public class TokenType
    {
        public int Id { get; set; }
        public string TokenId { get; set; }
        public string Token { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public interface ILogin
    {
        bool Login();
        int ExpirationTimeMinute();
        string Token();
        void LoadToken(string Token);
        string TokenId();
    }

    public interface IToken
    {
        string ProviderId();
    }

    public interface ITokenData
    {
        TokenType GetToken(string TokenId);
        int InsertToken(TokenType val);

    }
}
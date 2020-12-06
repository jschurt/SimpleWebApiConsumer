using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleWebApiConsumer.Models;

namespace SimpleWebApiConsumer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        } //index


        #region Call generic WebAPIs (get)

        [HttpGet]
        public async Task<IActionResult> Process(string errorMsg)
        {

            ProcessApiViewModel model = new ProcessApiViewModel() { EndPointReturn = errorMsg };

            return View(model);

        } //index

        [HttpPost]
        public async Task<IActionResult> Process(ProcessApiViewModel model)
        {


            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(model.EndPointUrl))
                    {

                        model.EndPointReturn = await response.Content.ReadAsStringAsync();

                        //E se quisermos deserializar o json?
                        //Original : (Usando NewtonSoft.Json)
                        //var result =  JsonConvert.DeserializeObject<List<minhaClasse>>(apiResponse);

                        //Original : (Usando System.Txt.Json)
                        //var result = await JsonSerializer.DeserializeAsync<List<minhaClasse>>(apiResponse);

                    }
                }

            }
            catch (Exception e)
            {
                model.EndPointReturn = e.Message;
            }

            return View(model);

        } //index

        #endregion

        #region Call JWT Protected WebAPI


        /// <summary>
        /// 1. Para chamar uma WebAPI protegida com token, o primeiro passo eh se autenticar na webapi que deve possuir algum metodo de autenticacao.
        /// 2. Apos chamar o endpoint de autenticacao, receberemos um Bearer Token que deveremos usar para chamar os outros metodos. 
        /// 
        /// Obs. Para esta simulacao, chamaremos o projeto "SuperSimpleAPIJWT", que eh um webapi de teste protegido com JWT.
        /// 
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<string> getToken() {


            string token = "";

            //O projeto "SuperSimpleAPIJWT" deve estar em execucao nesta url. O endpoit abaixo eh responsavel pela autenticacao.
            //Os usuarios aceitaveis sao "Julio" e "Gabriel"
            var authenticationEndPoint = "https://localhost:44348/v1/account/login";
            var uri = new Uri(string.Format(authenticationEndPoint));

            //Preparando credenciais para encaminhar por post
            User credentials = new User() { UserName = "Julio", Password = "123" };
            var data = JsonSerializer.Serialize(credentials);  
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                using (var httpClient = new HttpClient())
                {


                    using (var response = await httpClient.PostAsync(uri, content))
                    {

                        var json = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<AuthenticationTokenViewModel>(json);

                        token = result.Token;

                    }
                }

            }
            catch (Exception e)
            {
                token = "Fail to get token";
            }

            return token;

        }

        [HttpGet]
        public async Task<IActionResult> ProcessApiJWT(string errorMsg)
        {

            ProcessApiViewModel model = new ProcessApiViewModel() { EndPointReturn = errorMsg, EndPointUrl = "https://localhost:44348/v1/account/employee", Token = await getToken() };

            return View(model);

        } //index


        [HttpPost]
        public async Task<IActionResult> ProcessApiJWT(ProcessApiViewModel model)
        {

            var uri = new Uri(string.Format(model.EndPointUrl));

            try
            {
                using (var httpClient = new HttpClient())
                {


                    //Incluindo token no header
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", model.Token);

                    using (var response = await httpClient.GetAsync(model.EndPointUrl))
                    {
                        model.ResultStatusCode = response.StatusCode.ToString();
                        model.EndPointReturn = await response.Content.ReadAsStringAsync();

                    }
                }

            }
            catch (Exception e)
            {
                model.EndPointReturn = e.Message;
            }

            return View(model);

        } //index


        #endregion


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

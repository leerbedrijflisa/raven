using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lisa.Raven.Checkers.DefaultCheckers.Controllers
{
    public class CheckController : ApiController
    {
        [HttpPost]
        public IHttpActionResult CheckHtml([FromUri] string v, [FromBody] ParsedHtml html)
        {
            int outcome = 0;

            foreach(var token in html.TokenStream)
            {
                if(token.Lexeme.StartsWith("<html"))
                {
                    outcome = 1;
                }
            }

            var errors = new List<ValidationError>();

            if(outcome == 0)
            {
                errors.Add(new ValidationError("A HTML tag is required."));
            }

            return Ok(errors);
 
        }
    }
}
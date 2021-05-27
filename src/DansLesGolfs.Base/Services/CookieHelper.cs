using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DansLesGolfs.Base
{
    public class CookieHelper : IDisposable
    {

        protected HttpRequest Request
        {
            get
            {
                return System.Web.HttpContext.Current.Request;
            }
        }
        protected HttpResponse Response
        {
            get
            {
                return System.Web.HttpContext.Current.Response;
            }
        }
        public CookieHelper()
        {
        }

        public bool AddCookie( string key, string value, DateTime expire )
        {
            try
            {
                if ( HasCookie( key ) )
                {
                    Response.Cookies[key].Value = value;
                }
                else
                {
                    HttpCookie cookie = new HttpCookie( key, value );
                    Response.Cookies.Add( cookie );
                }
                Response.Cookies[key].Expires = expire;
                return true;
            }
            catch ( Exception ex )
            {
                return false;
            }
        }

        public bool AddCookie( string key, Dictionary<string, string> values, DateTime expire )
        {
            try
            {
                if ( HasCookie( key ) )
                {
                    foreach ( KeyValuePair<string, string> entry in values )
                    {
                        if ( false == Response.Cookies[key].Values.AllKeys.Contains( entry.Key ) )
                        {
                            Response.Cookies[key].Values.Add( entry.Key, entry.Value );
                        }
                        else
                        {
                            Response.Cookies[key].Values[entry.Key] = entry.Value;
                        }

                    }
                }
                else
                {
                    HttpCookie cookie = new HttpCookie( key );
                    foreach ( KeyValuePair<string, string> entry in values )
                    {
                        Response.Cookies[key].Values.Add( entry.Key, entry.Value );

                    }
                }
                Response.Cookies[key].Expires = expire;
                return true;
            }
            catch ( Exception ex )
            {
                return false;
            }
        }

        public HttpCookie GetCookie( string key )
        {
            if ( HasCookie( key ) )
            {
                return Request.Cookies[key];
            }
            else
            {
                return null;
            }
        }

        public bool HasCookie( string key )
        {
            if ( Request.Cookies.AllKeys.Contains( key ) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DeleteCookie( string key )
        {
            if ( HasCookie( key ) )
            {
                HttpCookie cookie = new HttpCookie( key );
                cookie.Expires = DateTime.Now.AddDays( -1 );
                Response.Cookies.Add( cookie );
            }
        }

        public void Dispose()
        {
        }
    }
}

using System.Web.Mvc;

namespace TestDbFirst.Controllers
{
    /// <summary>  
    /// Home controller class.   
    /// </summary>  
    [Authorize]
    public class HomeController : Controller
    {
        #region Index method.  
        /// <summary>  
        /// Index method.   
        /// </summary>  
        /// <returns>Returns - Index view</returns>  
        public ActionResult Index()
        {
            return this.View();
        }
        #endregion
    }
}

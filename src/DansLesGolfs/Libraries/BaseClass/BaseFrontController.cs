using DansLesGolfs.Base;
using DansLesGolfs.Base.Services;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DansLesGolfs.Controllers
{
    public class BaseFrontController : BaseController
    {
        private int cacheLength = 86400; // Cache length in seconds
        private string adsContent = string.Empty;

        #region BeginExecuteCore
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {

            SqlDataAccess DataAccess = DataFactory.GetInstance();

            InitializeViewBags();

            return base.BeginExecuteCore(callback, state);
        }
        #endregion

        #region InitializeViewBags
        protected virtual void InitializeViewBags()
        {
            ViewBag.FBAppId = System.Configuration.ConfigurationManager.AppSettings["FBAppId"];
            LoadImportantData();
            LoadItemAttributesData();
            LoadTopNavIcons();
            LoadMainMenu();
            LoadFooterMenu();
            LoadAds();

            if (Auth.User != null)
            {
                int totalOrders = 0, totalMessages = 0, totalReferals = 0;
                DataAccess.GetUserNotifications(Auth.User.UserId, out totalOrders, out totalMessages, out totalReferals);
                ViewBag.TotalUserOrders = totalOrders;
                ViewBag.TotalUserMessage = totalMessages;
                ViewBag.TotalSponsorEmails = totalReferals;
            }
        }
        #endregion

        #region LoadImportantData
        protected void LoadImportantData()
        {
            int countryId = 0, siteId = 0, regionId = 0, stateId = 0, itemCategoryId = 0, itemSubCategoryId = 0, golfLessonCategoryId = 0;
            DateTime? fromDate = DateTime.Today, toDate = DateTime.Today.AddDays(1);
            int? timeSlot = null;
            bool includePractice = false, includeAccommodation = false;
            string departureMonth = string.Empty;

            ViewBag.CountryId = countryId = System.Web.HttpContext.Current.Request.Form["CountryId"] == null ? countryId : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["CountryId"]);
            ViewBag.SiteId = siteId = System.Web.HttpContext.Current.Request.Form["SiteId"] == null ? siteId : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["SiteId"]);
            ViewBag.RegionId = regionId = System.Web.HttpContext.Current.Request.Form["RegionId"] == null ? regionId : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["RegionId"]);
            ViewBag.StateId = stateId = System.Web.HttpContext.Current.Request.Form["StateId"] == null ? stateId : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["StateId"]);
            ViewBag.ItemCategoryId = itemCategoryId = System.Web.HttpContext.Current.Request.Form["ItemCategoryId"] == null ? itemCategoryId : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["ItemCategoryId"]);
            ViewBag.ItemSubCategoryId = itemSubCategoryId = System.Web.HttpContext.Current.Request.Form["ItemSubCategoryId"] == null ? itemSubCategoryId : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["ItemSubCategoryId"]);
            ViewBag.GolfLessonCategoryId = golfLessonCategoryId = System.Web.HttpContext.Current.Request.Form["GolfLessonCategoryId"] == null ? golfLessonCategoryId : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["GolfLessonCategoryId"]);
            ViewBag.TimeSlot = timeSlot = System.Web.HttpContext.Current.Request.Form["TimeSlot"] == null ? timeSlot : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["TimeSlot"]);
            ViewBag.IncludePractice = includePractice = System.Web.HttpContext.Current.Request.Form["IncludePractice"] == null ? includePractice : DataManager.ToBoolean(System.Web.HttpContext.Current.Request.Form["IncludePractice"]);
            ViewBag.IncludeAccommodation = includeAccommodation = System.Web.HttpContext.Current.Request.Form["IncludeAccommodation"] == null ? includeAccommodation : DataManager.ToBoolean(System.Web.HttpContext.Current.Request.Form["IncludeAccommodation"]);
            ViewBag.DepartureMonth = departureMonth = System.Web.HttpContext.Current.Request.Form["DepartureMonth"] == null ? departureMonth : DataManager.ToString(System.Web.HttpContext.Current.Request.Form["DepartureMonth"]);
            ViewBag.FromDate = fromDate = System.Web.HttpContext.Current.Request.Form["FromDate"] == null ? null : (DateTime?)DataManager.ToDateTime(System.Web.HttpContext.Current.Request.Form["FromDate"], "dd/MM/yyyy");
            ViewBag.ToDate = toDate = System.Web.HttpContext.Current.Request.Form["ToDate"] == null ? null : (DateTime?)DataManager.ToDateTime(System.Web.HttpContext.Current.Request.Form["ToDate"], "dd/MM/yyyy");

            // Countries
            InitDropDownCountries(countryId);

            // Regions
            InitDropDownRegions(countryId, regionId);

            // States
            InitDropDownStates(regionId, stateId);

            // Sites
            InitDropDownSites(siteId, stateId, regionId);

            ViewBag.GreenFeeCategories = LoadCacheValue<Dictionary<int, string>>("GreenFeeCategoriesMenu", () =>
            {
                return DataAccess.GetItemCategoriesKeyValuePairs((int)ItemType.Type.GreenFee);
            });

            ViewBag.DropDownGolfLessonCategories = LoadDropDownListCache("DropDownGolfLessonCategories", () =>
            {
                List<ItemCategory> golfLessonCategories = DataAccess.GetItemCategoriesDropDownList((int)ItemType.Type.GolfLesson);
                golfLessonCategories.Insert(0, new ItemCategory() { CategoryId = 0, CategoryName = Resources.Resources.SelectTypeOfLesson });
                return ListToDropDownList(golfLessonCategories, "CategoryId", "CategoryName", golfLessonCategoryId);
            }, itemCategoryId);

            List<ItemCategory> allItemCategories = LoadCacheValue<List<ItemCategory>>("DropDownItemCategories", () =>
            {
                List<ItemCategory> itemCategories = DataAccess.GetItemCategoriesDropDownList((int)ItemType.Type.Product);
                itemCategories.Insert(0, new ItemCategory() { CategoryId = 0, CategoryName = Resources.Resources.Categories });
                return itemCategories;
            });
            ViewBag.DropDownItemCategories = ListToDropDownList(allItemCategories.Where(it => it.ParentCategoryId == 0 || it.CategoryId == 0).ToList(), "CategoryId", "CategoryName", itemCategoryId);
            int parentItemCategoryId = itemCategoryId <= 0 ? -1 : itemCategoryId;
            List<SelectListItem> itemSubCategories = ListToDropDownList(allItemCategories.Where(it => it.ParentCategoryId == parentItemCategoryId || it.CategoryId == 0).ToList(), "CategoryId", "CategoryName", itemSubCategoryId);
            itemSubCategories.ForEach(it => { if (it.Value == "0") it.Text = Resources.Resources.SubCategory; });
            ViewBag.DropDownItemSubCategories = itemSubCategories;

            //ViewBag.TimeSlots = GetTimeSlotsDropDownList(timeSlot);

            ViewBag.DepartureMonths = GetDepartureMonths(departureMonth);
        }
        #endregion

        #region InitDropDownSites
        private void InitDropDownSites(long siteId, int stateId, int regionId)
        {
            List<Site> allSites = GetAllSites();
            List<Site> franceSite = GetFranceSites(allSites);
            if (stateId > 0)
            {
                ViewBag.DropDownSites = ViewBag.FranceSites = ListToDropDownList(allSites.Where(it => it.SiteId == 0 || (it.StateId == stateId && it.LangId == this.CultureId)).ToList(), "SiteId", "SiteName", siteId);
            }
            else if (regionId > 0)
            {
                ViewBag.DropDownSites = ViewBag.FranceSites = ListToDropDownList(allSites.Where(it => it.SiteId == 0 || (it.LangId == this.CultureId && it.RegionId == regionId)).ToList(), "SiteId", "SiteName");
            }
            else
            {
                ViewBag.FranceSites = ListToDropDownList(franceSite.Where(it => it.SiteId == 0 || (it.LangId == this.CultureId)).ToList(), "SiteId", "SiteName", siteId);
                ViewBag.DropDownSites = ListToDropDownList(allSites.Where(it => it.SiteId == 0 || (it.LangId == this.CultureId)).ToList(), "SiteId", "SiteName", siteId);
            }
        }

        private List<Site> GetFranceSites(List<Site> allSites)
        {
            List<Site> franceSites = LoadCacheValue<List<Site>>("DropDownFranceSites", () =>
            {
                return allSites.Where(it => it.SiteId == 0 || (it.LangId == this.CultureId && it.CountryId == 1)).ToList();
            });
            return franceSites;
        }

        private List<Site> GetAllSites()
        {
            List<Site> allSites = LoadCacheValue<List<Site>>("DropDownAllSites", () =>
            {
                List<Site> sites = DataAccess.GetAvailableDropDownSites();
                return sites.Where(it => it.Visible == true && it.Active == true).OrderBy(it => it.RegionId).OrderBy(it => it.SiteName).ToList();
            });

            Dictionary<int, int> availableSites = LoadCacheValue<Dictionary<int, int>>("DropDownSitesByItemType", () =>
            {
                return DataAccess.GetAvailableDropDownSitesByItemType();
            });

            allSites.Insert(0, new Site() { SiteId = 0, SiteName = Resources.Resources.SelectSite, Active = true, Visible = true });
            return allSites;
        }
        #endregion

        #region InitDropDownStates
        private void InitDropDownStates(int regionId, int stateId)
        {
            List<Tuple<int, int>> availableStates = LoadCacheValue<List<Tuple<int, int>>>("DropDownStatesByItemType", () =>
            {
                return DataAccess.GetAvailableStatesByItemType();
            });
            List<State> allStates = GetAllStates();
            List<State> greenFeeState = GetItemTypeStates(allStates, availableStates, ItemType.Type.GreenFee);
            List<State> stayFeeState = GetItemTypeStates(allStates, availableStates, ItemType.Type.StayPackage);
            List<State> stageFeeState = GetItemTypeStates(allStates, availableStates, ItemType.Type.GolfLesson);
            List<State> drivingFeeState = GetItemTypeStates(allStates, availableStates, ItemType.Type.DrivingRange);
            List<State> productFeeState = GetItemTypeStates(allStates, availableStates, ItemType.Type.Product);
            List<State> franceStates = GetFranceStates(allStates);

            if (regionId > 0)
            {
                ViewBag.DropDownStates = ViewBag.FranceStates = ListToDropDownList(allStates.Where(it => it.RegionId == regionId || it.StateId == 0).ToList(), "StateId", "StateName", stateId);
            }
            else
            {
                ViewBag.FranceStates = ListToDropDownList(franceStates, "StateId", "StateName", stateId);
                ViewBag.DropDownStates = ListToDropDownList(allStates, "StateId", "StateName", stateId);
            }
        }

        private List<State> GetItemTypeStates(List<State> allStates, List<Tuple<int, int>> availableStates, ItemType.Type type)
        {
            return allStates.Where(it => availableStates.Where(t => t.Item1 == (int)type).Distinct().Select(t => t.Item2).Contains(it.StateId) || it.StateId == 0).ToList();
        }

        private List<State> GetFranceStates(List<State> allStates)
        {
            List<State> franceStates = LoadCacheValue<List<State>>("DropDownFranceStates", () =>
            {
                List<Region> allRegions = GetAllRegions();
                return (from it in allStates
                        join re in allRegions on it.RegionId equals re.RegionId into rej
                        from re in rej.DefaultIfEmpty()
                        where it.StateId == 0 || (re != null && re.CountryId == 1)
                        select it).ToList();
            });
            return franceStates;
        }

        private List<State> GetAllStates()
        {
            List<State> allStates = LoadCacheValue<List<State>>("DropDownAllStates", () =>
            {
                List<State> states = DataAccess.GetAvailableStates();
                return states.OrderBy(it => it.RegionId).OrderBy(it => it.StateName).ToList();
            });

            allStates.Insert(0, new State() { StateId = 0, StateName = Resources.Resources.SelectState });
            return allStates;
        }
        #endregion

        #region InitDropDownRegions
        private void InitDropDownRegions(int countryId, int regionId)
        {
            List<Region> allRegions = GetAllRegions();
            List<Region> franceRegions = GetFranceRegions(allRegions);
            if (countryId > 0)
            {

                ViewBag.DropDownAllRegions = ListToDropDownList(allRegions.Where(it => it.CountryId == countryId || it.RegionId == 0).ToList(), "RegionId", "RegionName", regionId);
                ViewBag.DropDownRegionsByCountry = ListToDropDownList(allRegions.Where(it => it.CountryId == countryId || it.RegionId == 0).ToList(), "RegionId", "RegionName", regionId);
            }
            else
            {
                ViewBag.DropDownAllRegions = ListToDropDownList(allRegions, "RegionId", "RegionName", regionId);
                ViewBag.DropDownRegionsByCountry = ListToDropDownList(allRegions, "RegionId", "RegionName", regionId);
            }

            ViewBag.FranceRegions = ListToDropDownList(franceRegions, "RegionId", "RegionName", regionId);
        }

        private List<Region> GetFranceRegions(List<Region> allRegions)
        {
            List<Region> franceRegions = LoadCacheValue<List<Region>>("DropDownFranceRegions", () =>
            {
                return allRegions.Where(it => it.CountryId == 1 || it.RegionId == 0).ToList();
            });
            return franceRegions;
        }

        private List<Region> GetAllRegions()
        {
            List<Region> allRegions = LoadCacheValue<List<Region>>("DropDownAllRegions", () =>
            {
                List<Region> regions = DataAccess.GetAvailableRegions();
                return regions.OrderBy(it => it.CountryId).OrderBy(it => it.RegionName).ToList();
            });

            Dictionary<int, int> availableRegions = LoadCacheValue<Dictionary<int, int>>("DropDownRegionsByItemType", () =>
            {
                return DataAccess.GetAvailableRegionsByItemType();
            });

            allRegions.Insert(0, new Region() { RegionId = 0, RegionName = Resources.Resources.SelectRegion });
            return allRegions;
        }
        #endregion

        #region InitDropDownCountries
        private void InitDropDownCountries(int countryId)
        {

            List<Tuple<int, int>> availableCountries = LoadCacheValue<List<Tuple<int, int>>>("DropDownCountriesByItemType", () =>
            {
                return DataAccess.GetAvailableCountriesByItemType();
            });

            List<Country> allCountries = GetAllCountries();
            List<Country> stayCountries = GetStayCountries(allCountries, availableCountries);
            List<Country> stageCountries = GetStageCountries(allCountries, availableCountries);

            ViewBag.DropDownCountries = ListToDropDownList(allCountries, "CountryId", "CountryName", countryId);
            ViewBag.DropDownStayCountries = ListToDropDownList(stayCountries, "CountryId", "CountryName", countryId);
            ViewBag.DropDownStageCountries = ListToDropDownList(stageCountries, "CountryId", "CountryName", countryId);
        }

        private List<Country> GetStayCountries(List<Country> allCountries, List<Tuple<int, int>> availableCountries)
        {
            return allCountries.Where(it=>availableCountries.Where(t => t.Item1 == (int)ItemType.Type.StayPackage).Distinct().Select(t=>t.Item2).Contains(it.CountryId) || it.CountryId == 0).ToList();
        }

        private List<Country> GetStageCountries(List<Country> allCountries, List<Tuple<int, int>> availableCountries)
        {
            return allCountries.Where(it => availableCountries.Where(t => t.Item1 == (int)ItemType.Type.GolfLesson).Distinct().Select(t => t.Item2).Contains(it.CountryId) || it.CountryId == 0).ToList();
        }

        private List<Country> GetAllCountries()
        {
            List<Country> allCountries = LoadCacheValue<List<Country>>("DropDownCountries", () =>
            {
                return DataAccess.GetAvailableCountries();
            });
            allCountries.Add(new Country() { CountryId = 0, CountryName = Resources.Resources.SelectCountry });
            return allCountries;
        }
        #endregion

        #region GetDepartureMonths
        private List<SelectListItem> GetDepartureMonths(string selectedMonth)
        {
            List<SelectListItem> departureMonths = LoadCacheValue<List<SelectListItem>>("DepartureMonthsCache", () =>
            {
                List<SelectListItem> items = new List<SelectListItem>();
                for (DateTime i = DateTime.Today, j = DateTime.Today.AddYears(2); i < j; i = i.AddMonths(1))
                {
                    items.Add(new SelectListItem()
                    {
                        Text = i.ToString("MMMM yyyy"),
                        Value = i.ToString("yyyyMM"),
                        Selected = (i.ToString("yyyyMM") == selectedMonth)
                    });
                }
                return items;
            });
            departureMonths.Add(new SelectListItem()
            {
                Text = Resources.Resources.SelectMonth,
                Value = "0",
                Selected = (string.IsNullOrEmpty(selectedMonth) || string.IsNullOrWhiteSpace(selectedMonth))
            });
            departureMonths.ForEach(it => { it.Selected = it.Value == selectedMonth; });

            return departureMonths;
        }
        #endregion

        #region GetTimeSlotsDropDownList
        private List<SelectListItem> GetTimeSlotsDropDownList(int? timeSlot = null)
        {
            timeSlot = timeSlot.HasValue ? timeSlot.Value : 0;
            List<SelectListItem> timeSlots = LoadCacheValue<List<SelectListItem>>("TimeSlotCache", () =>
            {
                List<SelectListItem> slots = new List<SelectListItem>();
                for (int i = 7; i < 20; i++)
                {
                    slots.Add(new SelectListItem()
                    {
                        Text = i.ToString("00") + "h00-" + (i + 1).ToString("00") + "h00",
                        Value = i.ToString()
                    });
                }
                return slots;
            });
            timeSlots.Add(new SelectListItem()
            {
                Text = Resources.Resources.TimeSlot,
                Value = "0",
                Selected = timeSlot == null
            });
            timeSlots.ForEach(it => { it.Selected = timeSlot.ToString() == it.Value; });
            return timeSlots;
        }
        #endregion

        #region LoadAllOptions
        protected void LoadAllOptions()
        {
            Options = DataAccess.GetOptions();
        }
        #endregion

        #region LoadItemAttributesData
        protected void LoadItemAttributesData()
        {
            int dexterityId = 0, shaftId = 0, levelId = 0;
            string shapeIds = "", genreIds = "";
            ViewBag.SearchShapeIds = shapeIds = System.Web.HttpContext.Current.Request.Form["SearchShapeIds"] == null ? string.Empty : System.Web.HttpContext.Current.Request.Form["SearchShapeIds"].ToString();
            ViewBag.SearchGenreIds = genreIds = System.Web.HttpContext.Current.Request.Form["SearchGenreIds"] == null ? string.Empty : System.Web.HttpContext.Current.Request.Form["SearchGenreIds"].ToString();
            ViewBag.SearchDexterityId = dexterityId = System.Web.HttpContext.Current.Request.Form["SearchDexterityId"] == null ? 0 : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["SearchDexterityId"]);
            ViewBag.SearchShaftId = shaftId = System.Web.HttpContext.Current.Request.Form["SearchShaftId"] == null ? 0 : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["SearchShaftId"]);
            ViewBag.SearchLevelId = levelId = System.Web.HttpContext.Current.Request.Form["SearchLevelId"] == null ? 0 : DataManager.ToInt(System.Web.HttpContext.Current.Request.Form["SearchLevelId"]);

            List<ModifierChoice> choices = DataAccess.GetModifierChoicesByModifierNames("Dexterity,Genre,Shaft,Shape,Level", this.CultureId);


            List<ModifierChoice> shapes = GetModifierChoicesByModifierName("Shape", choices);
            ViewBag.FilterShapes = shapes.OrderBy(it => it.ChoiceId).ToList();

            List<ModifierChoice> genres = GetModifierChoicesByModifierName("Genre", choices);
            ViewBag.FilterGenres = genres.OrderBy(it => it.ChoiceId).ToList();

            List<ModifierChoice> levels = GetModifierChoicesByModifierName("Level", choices);
            levels.Insert(0, new ModifierChoice { ChoiceId = 0, ChoiceName = Resources.Resources.Level });
            ViewBag.DropDownLevels = ListToDropDownList(levels, "ChoiceId", "ChoiceName", levelId);

            List<ModifierChoice> dexterities = GetModifierChoicesByModifierName("Dexterity", choices);
            dexterities.Insert(0, new ModifierChoice { ChoiceId = 0, ChoiceName = Resources.Resources.Dexterity });
            ViewBag.DropDownDexterities = ListToDropDownList(dexterities, "ChoiceId", "ChoiceName", dexterityId);

            List<ModifierChoice> shafts = GetModifierChoicesByModifierName("Shaft", choices);
            shafts.Insert(0, new ModifierChoice { ChoiceId = 0, ChoiceName = Resources.Resources.Material });
            ViewBag.DropDownShafts = ListToDropDownList(shafts, "ChoiceId", "ChoiceName", shaftId);
        }
        #endregion

        #region LoadMainMenu
        private void LoadMainMenu()
        {
            if (MemoryCache.Default["DLGMainMenu"] == null)
            {
                Menu menu = DataAccess.GetMenuByPlacement("main-menu");
                MvcHtmlString html = MvcHtmlString.Create(GetMenuHTML(menu));
                MemoryCache.Default.Add("DLGMainMenu", html, DateTime.Now.AddYears(1));
                ViewBag.DLGMainMenu = html;
            }
            else
            {
                ViewBag.DLGMainMenu = MemoryCache.Default["DLGMainMenu"] as MvcHtmlString;
            }
        }
        private void LoadFooterMenu()
        {
            if (MemoryCache.Default["DLGFooterMenu"] == null)
            {
                Menu menu = DataAccess.GetMenuByPlacement("footer-menu");
                MvcHtmlString html = MvcHtmlString.Create(GetMenuHTML(menu));
                MemoryCache.Default.Add("DLGFooterMenu", html, DateTime.Now.AddYears(1));
                ViewBag.DLGFooterMenu = html;
            }
            else
            {
                ViewBag.DLGFooterMenu = MemoryCache.Default["DLGFooterMenu"] as MvcHtmlString;
            }
        }

        private string GetMenuHTML(Menu menu)
        {
            if (menu == null || menu.MenuItems == null || !menu.MenuItems.Any())
                return string.Empty;

            string html = string.Empty;

            html = "<ul>" + GetHTMLByMenuItemList(menu.MenuItems) + "</ul>";

            return html;
        }

        private string GetHTMLByMenuItemList(List<MenuItem> list, string prefix = "", string suffix = "")
        {
            string html = string.Empty,
                type = string.Empty,
                id = string.Empty;

            foreach (MenuItem item in list)
            {
                if (item.MenuType.Equals("link", StringComparison.InvariantCultureIgnoreCase))
                {
                    type = item.MenuType.ToLower().Trim();
                    id = item.MenuItemId.ToString();
                }
                else
                {
                    type = item.MenuType.ToLower().Trim();
                    id = item.MenuValue.Trim();
                }

                html += "<li class=\"";
                html += type + " " + type + "-" + id + "\">";
                html += prefix;
                html += "<a href=\"" + GetMenuUrl(item) + "\"";
                if (item.IsOpenNewWindow)
                {
                    html += " target=\"_blank\"";
                }
                html += ">";
                html += item.MenuTitle + "</a>";
                html += suffix;
                if (item.Children.Any())
                {
                    html += "<ul class=\"submenu\">";
                    html += GetHTMLByMenuItemList(item.Children, "<i class=\"glyphicon glyphicon-play\"></i> ");
                    html += "</ul>";
                }
                html += "</li>";
            }
            return html;
        }

        private string GetMenuUrl(MenuItem item)
        {
            switch (item.MenuType.ToLower().Trim())
            {
                case "itemtype":
                    string itemTypeName = Enum.GetName(typeof(ItemType.Type), DataManager.ToInt(item.MenuValue));
                    return Url.Content("~/" + itemTypeName);
                case "category":
                    return Url.Content("~/GreenFee?cat=" + item.MenuValue);
                default:
                    return item.MenuValue.Trim();
            }
        }
        #endregion

        #region LoadAds
        private void LoadAds()
        {
            if (MemoryCache.Default["DLGAds"] == null)
            {
                List<Ad> ads = DataAccess.GetAllAds();
                MemoryCache.Default.Add("DLGAds", ads, DateTime.Now.AddDays(1));
                ViewBag.DLGAds = ads;
            }
            else
            {
                ViewBag.DLGAds = MemoryCache.Default["DLGAds"] as List<Ad>;
            }
        }
        #endregion

        #region LoadAdsList
        protected void LoadAdsList(string adsetName)
        {
            string htmlStr = string.Empty;
            if (MemoryCache.Default["DLGAds"] == null)
                LoadAds();

            if (MemoryCache.Default["DLGAds"] != null)
            {
                List<Ad> ads = MemoryCache.Default["DLGAds"] as List<Ad>;
                if (ads != null)
                {
                    htmlStr = "<ul class=\"widget-ads-list\">";
                    var result = from it in ads
                                 where (it.AdsetName != null && it.AdsetName.ToLower() == adsetName.ToLower()) && (it.FromDate <= DateTime.Today && it.ToDate >= DateTime.Today)
                                 select it;

                    foreach (var ad in result)
                    {
                        htmlStr += "<li class=\"ads ads-" + ad.AdsId + "\">";
                        htmlStr += "<a href=\"" + ad.LinkUrl + "\">";
                        htmlStr += "<img src=\"" + ad.ImageUrl + "\" />";
                        htmlStr += "</a></li>";
                    }

                    htmlStr += "</ul>";
                }
            }
            adsContent += htmlStr;
            CreateAdsHTML();
        }
        #endregion

        #region LoadTopNavIcons
        private void LoadTopNavIcons()
        {
            if (MemoryCache.Default["DLGTopNavLinks"] == null)
            {
                List<TopNavLink> topNavLinks = DataAccess.GetAllTopNavLinks();
                MemoryCache.Default.Add("DLGTopNavLinks", topNavLinks, DateTime.Now.AddYears(1));
                ViewBag.DLGTopNavLinks = topNavLinks;
            }
            else
            {
                ViewBag.DLGTopNavLinks = MemoryCache.Default["DLGTopNavLinks"] as List<TopNavLink>;
            }
        }
        #endregion

        #region CreateAdsHTML
        private void CreateAdsHTML()
        {
            ViewBag.AdsContent = MvcHtmlString.Create(adsContent);
        }
        #endregion

        #region LoadDropDownListCache
        private List<SelectListItem> LoadDropDownListCache(string cacheName, Func<List<SelectListItem>> loadFunction, object value = null)
        {
            if (loadFunction == null)
                return new List<SelectListItem>();

            InMemoryCache cache = new InMemoryCache("WebSiteCache");

            if (cache.Get(cacheName) != null)
            {
                List<SelectListItem> list = MemoryCache.Default[cacheName] as List<SelectListItem>;
                if (list != null && value != null)
                {
                    list.Where(it => it.Value == value.ToString()).ToList().ForEach(it => it.Selected = true);
                }
                return list;
            }
            else
            {
                List<SelectListItem> list = loadFunction();
                MemoryCache.Default.Add(cacheName, list, DateTime.Now.AddMinutes(20));
                return list;
            }
        }
        #endregion

        #region LoadCacheValue
        private T LoadCacheValue<T>(string cacheName, Func<T> loadFunction)
        {
            if (loadFunction == null)
                return (T)Activator.CreateInstance(typeof(T));

            InMemoryCache cache = new InMemoryCache("WebSiteCache");

            try
            {
                if (cache.Get(cacheName) != null)
                {
                    T obj = (T)MemoryCache.Default[cacheName];
                    if (obj == null)
                    {
                        obj = (T)Activator.CreateInstance(typeof(T));
                        MemoryCache.Default.Add(cacheName, obj, DateTime.Now.AddSeconds(cacheLength));
                    }
                    return obj;
                }
                else
                {
                    T obj = loadFunction();
                    MemoryCache.Default.Add(cacheName, obj, DateTime.Now.AddSeconds(cacheLength));
                    return obj;
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
        #endregion
    }

}

using WebApplication1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Helpers
{
    public class MenuHelper
    {

        public async Task<IList<Menu>> GetAllMenuItems(string route, string id)
        {

            var routeparts = new Uri(route);
            var routetype = "";
            int intid = 0;
            foreach (var segment in routeparts.Segments)
            {
                var segmentnoslash = "";
                int lastSlash = segment.LastIndexOf('/');
                segmentnoslash = (lastSlash > -1) ? segment.Substring(0, lastSlash) : segment;


                if (int.TryParse(segmentnoslash, out intid))
                {
                    continue;
                }
                switch (segmentnoslash.ToLower())
                {
                    case "":
                        routetype = "home";
                        break;

                    case "about":
                        routetype = "about";
                        break;

                    case "contact":
                        routetype = "contact";
                        break;

                    case "privacy":
                        routetype = "privacy";
                        break;
                        //Add new route names here
                }
            }
            if (intid == 0)
            {
                int.TryParse(id, out intid);
            }

            var menu = new List<Menu>();
            //var i = 0;

            //NOTE:  Make sure to give each unique element a different guid.
            
            //Lone parent item
            menu.Add(new Menu
            {
                ID = Guid.NewGuid(),
                ParentID = null,
                Content = "Overview",
                Order = 0, // i++,
                IconClass = "fa fa-fw fa-user",
                SelectedClass = (routetype == "home") ? "is-active" : null,
                Url = "/" + id// cbaId
            });

            //Parent item along with linked child items
            var parentguid = Guid.NewGuid();
            menu.Add(new Menu
            {
                ID = parentguid,
                ParentID = null,
                Content = "Items",
                Order = 1,
                IconClass = "fa fa-fw fa-folder-open",
                Url = "#"
            });
            menu.Add(new Menu
            {
                ID = Guid.NewGuid(),
                ParentID = parentguid,
                Content = "About",
                Order = 0,
                IconClass = "fa fa-fw fa-pie-chart",
                SelectedClass = (routetype == "about") ? "is-active" : null,
                Url = "/home/About"
            });
            menu.Add(new Menu
            {
                ID = Guid.NewGuid(),
                ParentID = parentguid,
                Content = "Contact",
                Order = 1,
                IconClass = "fa fa-fw fa-search",
                SelectedClass = (routetype == "contact") ? "is-active" : null,
                Url = "/home/Contact"
            });

            menu.Add(new Menu
            {
                ID = Guid.NewGuid(),
                ParentID = parentguid,
                Content = "Privacy",
                Order = 2,
                IconClass = "fa fa-fw fa-industry",
                SelectedClass = (routetype == "privacy") ? "is-active" : null,
                Url = "/home/Privacy"
            });

            //Parent item that calls JS code - Set the Onclick property as below
            menu.Add(new Menu
            {
                ID = Guid.NewGuid(),
                ParentID = null,
                Content = "Create New",
                Order = 2,
                IconClass = "fa fa-fw fa-plus",
                Url = "",
                OnClick = "CreateNewItem();"  //call JS function if you set the onclick property.
            });

            return menu;
        }


        public IList<MenuViewModel> GetMenu(IList<Menu> menuList, Guid? parentId)
        {
            var children = GetChildrenMenu(menuList, parentId);

            if (!children.Any())
            {
                return new List<MenuViewModel>();
            }

            var vmList = new List<MenuViewModel>();
            foreach (var item in children)
            {
                var menu = GetMenuItem(menuList, item.ID);
                var vm = new MenuViewModel();
                vm.ID = menu.ID;
                vm.Content = menu.Content;
                vm.IconClass = menu.IconClass;
                vm.Url = menu.Url;
                vm.SelectedStyle = menu.SelectedStyle;
                vm.SelectedClass = menu.SelectedClass;
                vm.OnClick = menu.OnClick;
                vm.Children = GetMenu(menuList, menu.ID);
                vmList.Add(vm);
            }
            return vmList;
        }


        private IList<Menu> GetChildrenMenu(IList<Menu> menuList, Guid? parentId = null)
        {
            return menuList.Where(x => x.ParentID == parentId).OrderBy(x => x.Order).ToList();
        }

        private Menu GetMenuItem(IList<Menu> menuList, Guid id)
        {
            return menuList.FirstOrDefault(x => x.ID == id);
        }

    }



}

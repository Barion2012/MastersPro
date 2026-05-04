using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Agregator.Models
{

    public class MenuItem
    {
        public string text { get; set; }
        public bool disabled { get; set; }
        public string icon { get; set; }
        public int price { get; set; }
        public IEnumerable<MenuItem> items { get; set; }
    }

    public static class MenuData
    {
        public static readonly IEnumerable<MenuItem> userMenu = new[] {
            new MenuItem {
                text = "userMenu",
                items = new[] {
                    new MenuItem {
                        text = "Мой профиль",
                        price = 220,
                        icon = "../../images/ProductsLarge/1.png"
                    },
                    new MenuItem {
                        text = "Выход",
                        price = 270,
                        icon = "../../images/ProductsLarge/2.png"
                    }
                }
            }
        };
    }
}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {

    public class UIMenuController : MonoSingleton<UIMenuController> {

        public Transform _uiRoot;
        private Stack<UIMenu> menus = new Stack<UIMenu>();

        public int MenuCount {
            get {
                return menus.Count;
            }
        }

        public T Push<T>(Transform uiParent)where T : UIMenu {
            return Push<T>(typeof(T).Name, uiParent);
        }

        public T Push<T>(string menuID, Transform uiParent)where T : UIMenu {
            UIMenu activeMenu = null;
            if (menus.Count > 0) {
                activeMenu = menus.Peek();
            }

            T newMenu = LoadResource<T>(menuID);
            if (activeMenu != null) {
                activeMenu.Deactivated();
                activeMenu.gameObject.SetActive(false);
            }

            menus.Push(newMenu);
            newMenu.MenuID = menuID;
            newMenu.Initialise(activeMenu);
            newMenu.Activated();

            newMenu.transform.SetParent(uiParent != null ? uiParent : _uiRoot, false);
            newMenu.transform.LocalZero();

            return newMenu;
        }

        public void Pop(UIMenu menu) {
            Requires.True(menus.Count > 0, "Failed to pop active menu, no menus active.");
            Requires.True(menu == menus.Peek(), "Failed to pop active menu. Trying to pop the non active menu");

            menus.Pop();
            menu.Deactivated();
            menu.Cleanup();
            Objects.DestroySingle(menu.gameObject);

            if (menus.Count <= 0) {
                return;
            }

            UIMenu activeMenu = menus.Peek();
            activeMenu.gameObject.SetActive(true);
            activeMenu.Activated();
        }

        public void Clear() {
            while (menus.Count > 0) {
                Pop(menus.Peek());
            }
        }

        public bool IsActiveMenu<T>()where T : UIMenu {
            return IsActiveMenu(typeof(T).Name);
        }

        public bool IsActiveMenu(string menuID) {
            if (menus.Count <= 0) {
                return false;
            }

            var activeMenu = menus.Peek();
            if (activeMenu.MenuID != menuID) {
                return false;
            }

            return true;
        }

        private T LoadResource<T>(string menuID)where T : UIMenu {
            string path = $"UI/{menuID}";
            T rawResource = Resources.Load<T>(path);
            Requires.NotNull(rawResource, $"Failed to menu resource at {path}");

            T typedInstance = Objects.CreateSingle(rawResource);
            return typedInstance;
        }

        public void Show() {
            Requires.True(menus.Count > 0);
            menus.Peek().gameObject.SetActive(true);
        }

        public void Hide() {
            Requires.True(menus.Count > 0);
            menus.Peek().gameObject.SetActive(false);
        }

        public T GetActiveMenu<T>()where T : UIMenu {
            Requires.True(menus.Count > 0);
            return menus.Peek()as T;
        }
    }

}
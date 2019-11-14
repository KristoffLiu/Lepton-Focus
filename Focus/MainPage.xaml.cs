using Focus.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Focus
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            #region 标题栏设置 Title Bar Setting
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true; //扩展标题栏
            Window.Current.SetTitleBar(TitleBar); //设置某个容器空间（我一般就用的Grid）为标题栏，即可拖拽来位移窗体
            //注意：该控件必须要有颜色，哪怕是Transparent透明也行。如果不设置background，它就是空的没法进行拖拽。
            var view = ApplicationView.GetForCurrentView();//来进行以下一系列设置标题栏组件颜色的操作

            view.TitleBar.ButtonBackgroundColor = Color.FromArgb(0, 0, 0, 0);
            view.TitleBar.ButtonForegroundColor = Colors.White;
            view.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(38, 0, 0, 0);
            view.TitleBar.ButtonHoverForegroundColor = Colors.White;
            view.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(70, 0, 0, 0);
            view.TitleBar.ButtonPressedForegroundColor = Colors.White;
            view.TitleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0, 0, 0, 0);
            view.TitleBar.ButtonInactiveForegroundColor = Colors.White;
            #endregion
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(350, 500));

            MainFrame.Navigate(typeof(FramePage));
        }

    }
}

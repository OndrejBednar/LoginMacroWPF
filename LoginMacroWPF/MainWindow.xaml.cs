﻿using LoginMacroWPF.Viewmodels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LoginMacroWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            PlatformFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            switch (App.CredentialRW.GetNotEmptyPlatform())
            {
                case Models.Platforms.Lol:
                    PlatformFrame.Navigate(new System.Uri("LeaguePage.xaml", System.UriKind.RelativeOrAbsolute));
                    break;
                case Models.Platforms.Steam:
                    PlatformFrame.Navigate(new System.Uri("SteamPage.xaml", System.UriKind.RelativeOrAbsolute));
                    break;
                case Models.Platforms.BattleNet:
                    break;
                default:
                    break;
            }
            //if (RiotAPI.Api == null) InvalidApiKey.Visible = true;
        }


        private void LeagueClicked(object sender, RoutedEventArgs e)
        {
            PlatformFrame.Navigate(new System.Uri("LeaguePage.xaml", System.UriKind.RelativeOrAbsolute));
        }

        private void SteamClicked(object sender, RoutedEventArgs e)
        {
            PlatformFrame.Navigate(new System.Uri("SteamPage.xaml", System.UriKind.RelativeOrAbsolute));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (rect.Visibility == System.Windows.Visibility.Collapsed)
            {
                rect.Visibility = System.Windows.Visibility.Visible;
                (sender as Button).Content = "<";
            }
            else
            {
                rect.Visibility = System.Windows.Visibility.Collapsed;
                (sender as Button).Content = ">";
            }
        }
    }
}

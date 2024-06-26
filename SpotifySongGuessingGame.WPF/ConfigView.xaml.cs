﻿using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotifySongGuessingGame.WPF
{
	/// <summary>
	/// Interaction logic for ConfigView.xaml
	/// </summary>
	public partial class ConfigView : Window
	{
		private readonly ConfigManager configManager;

		public ConfigView(ConfigManager configManager)
		{
			InitializeComponent();
			this.configManager = configManager;

			spotifyDatabaseLocationTextBox.Text = configManager.Get(ConfigKeys.DatabaseLocation);
			spotifyClientIdTextBox.Text = configManager.Get(ConfigKeys.SpotifyClientId);
			spotifyClientSecretTextBox.Text = configManager.Get(ConfigKeys.SpotifyClientSecret);
			musicBrainzIdTextBox.Text = configManager.Get(ConfigKeys.MusicbrainzLoginId);
			musicBrainzPasswordTextBox.Password = configManager.Get(ConfigKeys.MusicbrainzLoginPassword);
		}

		private void SaveButtonClicked(object sender, RoutedEventArgs e)
		{
			configManager.Set(ConfigKeys.DatabaseLocation, spotifyDatabaseLocationTextBox.Text);
			configManager.Set(ConfigKeys.SpotifyClientId, spotifyClientIdTextBox.Text);
			configManager.Set(ConfigKeys.SpotifyClientSecret, spotifyClientSecretTextBox.Text);
			configManager.Set(ConfigKeys.MusicbrainzLoginId, musicBrainzIdTextBox.Text);
			configManager.Set(ConfigKeys.MusicbrainzLoginPassword, musicBrainzPasswordTextBox.Password);
			Close();
		}

		private void DatabaseFolderSelectClicked(object sender, RoutedEventArgs e)
		{

			var picker = new CommonOpenFileDialog();
			picker.IsFolderPicker = true;

			var result = picker.ShowDialog();
			if (result == CommonFileDialogResult.Ok)
			{
				spotifyDatabaseLocationTextBox.Text = picker.FileName;
			}
		}
	}
}

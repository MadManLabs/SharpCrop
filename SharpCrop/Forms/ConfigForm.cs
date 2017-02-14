﻿using System;
using System.Linq;
using System.Windows.Forms;
using SharpCrop.Utils;

namespace SharpCrop.Forms
{
    /// <summary>
    /// ConfigForm is a UI for config modification.
    /// </summary>
    public partial class ConfigForm : Form
    {
        private readonly Controller controller;

        /// <summary>
        /// Construct a new ConfigForm and initialize it with ConfigHelper.
        /// </summary>
        public ConfigForm(Controller controller)
        {
            this.controller = controller;

            InitializeComponent();

            // Update add/remove provider list
            UpdateProviderList();

            // Init lists and boxes
            formatBox.Text = ConfigHelper.Memory.SafeImageFormat;
            formatBox.Items.AddRange(Constants.AvailableImageFormats.Keys.ToArray());
            videoFpsBox.Text = ConfigHelper.Memory.SafeVideoFps.ToString();
            videoFpsBox.Items.AddRange(Constants.AvailableFps.ToArray());
            urlToCopyBox.Text = ConfigHelper.Memory.ProviderToCopy;
            manualScallingBox.Text = string.Join(" ", ConfigHelper.Memory.SafeManualScaling);

            // Init checkboxes
            noCopyCheckBox.Checked = ConfigHelper.Memory.NoCopy;
            noScalingCheckBox.Checked = ConfigHelper.Memory.NoAutoScaling;
            noGifRepeatCheckBox.Checked = ConfigHelper.Memory.NoGifRepeat;
            noFocusCheckBox.Checked = ConfigHelper.Memory.NoFocus;
            noTransparencyCheckBox.Checked = ConfigHelper.Memory.NoTransparency;
            enableMpegCheckbox.Checked = ConfigHelper.Memory.EnableMpeg;
            loadOnStartup.Checked = ConfigHelper.Memory.LoadOnStartup;

#if __MonoCS__
            noFocusCheckBox.Enabled = false;
#endif
        }
        
        /// <summary>
        /// Change image format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormatChanged(object sender, EventArgs e)
        {
            ConfigHelper.Memory.ImageFormat = formatBox.Text;
        }

        /// <summary>
        /// Change GIF FPS value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GifFpsChanged(object sender, EventArgs e)
        {
            try
            {
                ConfigHelper.Memory.VideoFps = int.Parse(videoFpsBox.Text);
            }
            catch
            {
                videoFpsBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Change URL to copy value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UrlToCopyChanged(object sender, EventArgs e)
        {
            ConfigHelper.Memory.ProviderToCopy = urlToCopyBox.Text;
        }

        /// <summary>
        /// Change manual scaling list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualScalingChanged(object sender, EventArgs e)
        {
            try
            {
                var list = manualScallingBox.Text
                    .Split(' ')
                    .Select(int.Parse)
                    .ToList();

                ConfigHelper.Memory.ManualScaling = list;
            }
            catch
            {
                // Ignored
            }
        }

        /// <summary>
        /// Disable or enable scaling checker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoScalingChanged(object sender, EventArgs e)
        {
            ConfigHelper.Memory.NoAutoScaling = noScalingCheckBox.Checked;
        }

        /// <summary>
        /// Disable or enable URL copy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoCopyChanged(object sender, EventArgs e)
        {
            ConfigHelper.Memory.NoCopy = noCopyCheckBox.Checked;
        }

        /// <summary>
        /// Disable or enable GIF repeat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoGifRepeatChanged(object sender, EventArgs e)
        {
            ConfigHelper.Memory.NoGifRepeat = noGifRepeatCheckBox.Checked;
        }

        /// <summary>
        /// Disable or enable focus on launch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoFocusChanged(object sender, EventArgs e)
        {
            ConfigHelper.Memory.NoFocus = noFocusCheckBox.Checked;
        }

        /// <summary>
        /// Disable or enable transparency.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoTransparencyChanged(object sender, EventArgs e)
        {
            ConfigHelper.Memory.NoTransparency = noTransparencyCheckBox.Checked;
        }

        /// <summary>
        /// Disable or enable mpeq recording.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableMpegChanged(object sender, EventArgs e)
        {
            ConfigHelper.Memory.EnableMpeg = enableMpegCheckbox.Checked;
        }

        /// <summary>
        /// Disable or enable the load of the providers on startup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoadOnStartup(object sender, EventArgs e)
        {
            ConfigHelper.Memory.LoadOnStartup = loadOnStartup.Checked;
        }

        /// <summary>
        /// Reset application settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetClicked(object sender, EventArgs e)
        {
            ConfigHelper.Reset();
            Application.Exit();
        }

        /// <summary>
        /// Update the add/remove provider list the reflect the current state.
        /// </summary>
        private void UpdateProviderList()
        {
            urlToCopyBox.Items.Clear();
            addProviderBox.Items.Clear();
            removeProviderBox.Items.Clear();

            foreach(var name in Constants.AvailableProviders.Keys)
            {
                if(!controller.LoadedProviders.Keys.Contains(name))
                {
                    addProviderBox.Items.Add(name);
                }
                else
                {
                    urlToCopyBox.Items.Add(name);
                    removeProviderBox.Items.Add(name);
                }
            }
        }

        /// <summary>
        /// Add new provider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddProvider(object sender, EventArgs e)
        {
            if(addProviderBox.SelectedItem == null)
            {
                return;
            }
            
            controller.RegisterProvider(addProviderBox.SelectedItem.ToString());
            addProviderBox.ClearSelected();
        }

        /// <summary>
        /// Remove existing provider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRemoveProvider(object sender, EventArgs e)
        {
            if (removeProviderBox.SelectedItem == null)
            {
                return;
            }

            controller.ClearProvider(removeProviderBox.SelectedItem.ToString());
            removeProviderBox.ClearSelected();
            UpdateProviderList();
        }

        /// <summary>
        /// Focus on MainForm when shown.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Focus();
        }

        /// <summary>
        /// Update provider list on activation.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            UpdateProviderList();
        }
    }
}

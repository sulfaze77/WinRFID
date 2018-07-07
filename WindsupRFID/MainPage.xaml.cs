using Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.Networking.Proximity;
using Windows.UI.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Media.Imaging;
using Pcsc;
using Pcsc.Common;
using NfcSample;
using static NfcSample.NfcUtils;
using Windows.Devices.SmartCards;
using System.Threading.Tasks;
using SDKTemplate;
using WindsupRFID;
using System.Globalization;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using Microsoft.Toolkit.Uwp.Helpers;






// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
// if error:
// AppxManifest.xml
//  <DeviceCapability Name = "proximity" />

namespace WindsupRFID
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        SmartCardReader m_cardReader;
        bool isNewTagNewSUP;
        int selN;

        ObservableCollection<SUPData> SUPCategory = new ObservableCollection<SUPData>();
        ObservableCollection<SUP> AwaySUP = new ObservableCollection<SUP>();
        ObservableCollection<SUP> StockSUP = new ObservableCollection<SUP>();


        public MainPage()
        {
            this.InitializeComponent();

            GetSaves();
            /*
            SUPCategory.Add(new SUPData("Rigide", 0, 15, 20, 15));
            SUPCategory.Add(new SUPData("Gonflable", 1, 15, 20, 15));
            SUPCategory.Add(new SUPData("Poly", 2, 8, 10, 8));
            SUPCategory.Add(new SUPData("Pro", 3, 20, 30, 20));
            SUPCategory.Add(new SUPData("Mega", 4, 10, 15, 10));
            */
            
            LV_Category.ItemsSource = SUPCategory;
            NS_Cat.ItemsSource = SUPCategory;
            LeftShow.ItemsSource = AwaySUP;
            StockShow.ItemsSource = StockSUP;

            TglBlockModifType.IsOn = false;
            LV_Category.Opacity = 0.5;
            LV_Category.IsEnabled = false;
            selN = 0;
            isNewTagNewSUP = false;
            InitNFC();
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);
        }

        void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            // TODO: This is the time to save app data in case the process is terminated

            Windows.Storage.ApplicationDataContainer localSettings =
                    Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue TypeSUP =
                    new Windows.Storage.ApplicationDataCompositeValue();
            TypeSUP.Clear();
            TypeSUP["nb"] = SUPCategory.Count;
            for (int i = 0; i < SUPCategory.Count; i++)
            {
                string I = i.ToString();
                string name = I + "NAME";
                string id = I + "ID";
                string demi = I + "DEMI";
                string hour = I + "HOUR";
                string supp = I + "SUPP";

                TypeSUP[name] = SUPCategory[i].Name.ToString();
                TypeSUP[id] = SUPCategory[i].ID.ToString();
                TypeSUP[demi] = SUPCategory[i].Demi.ToString();
                TypeSUP[hour] = SUPCategory[i].Hour.ToString();
                TypeSUP[supp] = SUPCategory[i].Supp.ToString();
            }
            localSettings.Values["TypeOfSUP"] = TypeSUP;

            Windows.Storage.ApplicationDataCompositeValue SUPStock =
                    new Windows.Storage.ApplicationDataCompositeValue();
            SUPStock.Clear();
            SUPStock["nb"] = StockSUP.Count;
            for (int i = 0; i < StockSUP.Count; i++)
            {
                string I = i.ToString();
                string type = I + "TYPE";
                string id = I + "ID";

                SUPStock[id] = StockSUP[i].getID();
                SUPStock[type] = StockSUP[i].strType;

            }
            localSettings.Values["StockSUP"] = SUPStock;

            Windows.Storage.ApplicationDataCompositeValue SUPAway =
                    new Windows.Storage.ApplicationDataCompositeValue();
            SUPAway.Clear();
            SUPAway["nb"] = AwaySUP.Count;
            for (int i = 0; i < AwaySUP.Count; i++)
            {
                string I = i.ToString();
                string type = I + "TYPE";
                string id = I + "ID";
                string time = I + "TIME";

                SUPAway[id] = AwaySUP[i].getID();
                SUPAway[type] = AwaySUP[i].strType;
                SUPAway[time] = AwaySUP[i].getDeparture();

            }
            localSettings.Values["AwaySUP"] = SUPAway;

        }

        private void GetSaves()
        {
            Windows.Storage.ApplicationDataContainer localSettings =
                   Windows.Storage.ApplicationData.Current.LocalSettings;


            if (localSettings.Values.ContainsKey("TypeOfSUP"))
            {
                Windows.Storage.ApplicationDataCompositeValue TypeSUP =
                    (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["TypeOfSUP"];

                if (TypeSUP.ContainsKey("nb"))
                {
                    int nb = (int)TypeSUP["nb"];

                    for (int i = 0; i < nb ; i++)
                    {// retrieve all
                        string I = i.ToString();
                        string name = I + "NAME";
                        string id = I + "ID";
                        string demi = I + "DEMI";
                        string hour = I + "HOUR";
                        string supp = I + "SUPP";

                        string Name = (string)TypeSUP[name];
                        int ID = Int32.Parse((string)TypeSUP[id]);
                        int Demi = Int32.Parse((string)TypeSUP[demi]);
                        int Hour = Int32.Parse((string)TypeSUP[hour]);
                        int Supp = Int32.Parse((string)TypeSUP[supp]);

                        SUPData nData = new SUPData(Name, ID, Demi, Hour, Supp);
                        SUPCategory.Add(nData);
                    }
                }
            }
            if (localSettings.Values.ContainsKey("StockSUP"))
            {
                Windows.Storage.ApplicationDataCompositeValue SUPStock =
                (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["StockSUP"];
                if (SUPStock.ContainsKey("nb"))
                {
                    int nb = (int)SUPStock["nb"];

                    for (int i = 0; i < nb ; i++)
                    {// retrieve all
                        string I = i.ToString();
                        string type = I + "TYPE";
                        string id = I + "ID";

                        string Type = (string)SUPStock[type];
                        Int64 ID = (Int64)SUPStock[id];

                        foreach (var tp in SUPCategory)
                        {
                            if (tp.Name == Type)
                            {
                                SUP nSUP = new SUP(ID, tp);
                                StockSUP.Add(nSUP);
                                break;
                            }
                        }
                    }
                }
            }
            if (localSettings.Values.ContainsKey("AwaySUP"))
            {
                Windows.Storage.ApplicationDataCompositeValue SUPAway =
                (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["AwaySUP"];
                if (SUPAway.ContainsKey("nb"))
                {
                    int nb = (int)SUPAway["nb"];

                    for (int i = 0; i < nb; i++)
                    {// retrieve all
                        string I = i.ToString();
                        string type = I + "TYPE";
                        string id = I + "ID";
                        string time = I + "TIME";


                        string Type = (string)SUPAway[type];
                        Int64 ID = (Int64)SUPAway[id];
                        DateTimeOffset left = (DateTimeOffset)SUPAway[time];
                        foreach (var tp in SUPCategory)
                        {
                            if (tp.Name == Type)
                            {
                                SUP nSUP = new SUP(ID, tp, left);
                                AwaySUP.Add(nSUP);
                                break;
                            }
                        }
                    }
                }
            }


        }

        private Int64 ToUID(string input)
        {
            Int64 ID = 0;
            input = input.Replace("-", "");
            ID = Int64.Parse(input, NumberStyles.HexNumber);
            return ID;
        }



        private async void InitNFC()
        {
            var deviceInfo = await SmartCardReaderUtils.GetFirstSmartCardReaderInfo(SmartCardReaderKind.Nfc);

            if (m_cardReader == null)
            {
                m_cardReader = await SmartCardReader.FromIdAsync(deviceInfo.Id);
                m_cardReader.CardAdded += CardReader_CardAdded;
                m_cardReader.CardRemoved += CardReader_CardRemoved;
            }
        }


        private async void ValidNewType_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(newDemi.Text, out int nDemi)
              && Int32.TryParse(newHour.Text, out int nHour)
              && Int32.TryParse(newSupp.Text, out int nSupp))
            {
                bool okay = true;
                foreach (var tp in SUPCategory)
                {
                    if (tp.Name == newName.Text)
                    {
                        okay = false;
                        await Notify("Nom déjà utilisé !");
                    }
                }
                if (okay)
                {
                    SUPCategory.Add(new SUPData(newName.Text, SUPCategory.Count, nDemi, nHour, nSupp));
                    newTyp.Flyout.Hide();
                }

            }

        }

        private void CardReader_CardRemoved(SmartCardReader sender, CardRemovedEventArgs args)
        {
            LogMessage("Card removed");
        }

        private async void CardReader_CardAdded(SmartCardReader sender, CardAddedEventArgs args)
        {
            await HandleCard(args.SmartCard, sender);
        }


        /// <summary>
        /// Sample code to hande a couple of different cards based on the identification process
        /// </summary>
        /// <returns>None</returns>
        private async Task HandleCard(SmartCard card, SmartCardReader sender)
        {
            string UID = "";
            Int64 id = 0;
            try
            {

                // Connect to the card
                using (SmartCardConnection connection = await card.ConnectAsync())
                {
                    // Try to identify what type of card it was
                    IccDetection cardIdentification = new IccDetection(card, connection);
                    await cardIdentification.DetectCardTypeAync();
                    LogMessage("Connected to card\r\nPC/SC device class: " + cardIdentification.PcscDeviceClass.ToString());
                    LogMessage("Card name: " + cardIdentification.PcscCardName.ToString());
                    LogMessage("ATR: " + BitConverter.ToString(cardIdentification.Atr));

                    if ((cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.StorageClass) &&
                        (cardIdentification.PcscCardName == Pcsc.CardName.MifareUltralightC
                        || cardIdentification.PcscCardName == Pcsc.CardName.MifareUltralight
                        || cardIdentification.PcscCardName == Pcsc.CardName.MifareUltralightEV1))
                    {
                        // Handle MIFARE Ultralight
                        MifareUltralight.AccessHandler mifareULAccess = new MifareUltralight.AccessHandler(connection);

                        // Each read should get us 16 bytes/4 blocks, so doing
                        // 4 reads will get us all 64 bytes/16 blocks on the card
                        for (byte i = 0; i < 4; i++)
                        {
                            byte[] response = await mifareULAccess.ReadAsync((byte)(4 * i));
                            LogMessage("Block " + (4 * i).ToString() + " to Block " + (4 * i + 3).ToString() + " " + BitConverter.ToString(response));
                        }

                        byte[] responseUid = await mifareULAccess.GetUidAsync();
                        UID = BitConverter.ToString(responseUid);
                        //await Notify("UID: " + BitConverter.ToString(responseUid));
                    }
                    else if (cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.MifareDesfire)
                    {
                        // Handle MIFARE DESfire
                        Desfire.AccessHandler desfireAccess = new Desfire.AccessHandler(connection);
                        Desfire.CardDetails desfire = await desfireAccess.ReadCardDetailsAsync();

                        LogMessage("DesFire Card Details:  " + Environment.NewLine + desfire.ToString());
                    }
                    else if (cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.StorageClass
                        && cardIdentification.PcscCardName == Pcsc.CardName.FeliCa)
                    {
                        // Handle Felica
                        LogMessage("Felica card detected");
                        var felicaAccess = new Felica.AccessHandler(connection);
                        var uid = await felicaAccess.GetUidAsync();
                        UID = BitConverter.ToString(uid);
                        //await Notify("UID: " + BitConverter.ToString(uid));
                    }
                    else if (cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.StorageClass
                        && (cardIdentification.PcscCardName == Pcsc.CardName.MifareStandard1K || cardIdentification.PcscCardName == Pcsc.CardName.MifareStandard4K))
                    {
                        // Handle MIFARE Standard/Classic
                        LogMessage("MIFARE Standard/Classic card detected");
                        var mfStdAccess = new MifareStandard.AccessHandler(connection);
                        var uid = await mfStdAccess.GetUidAsync();
                        UID = BitConverter.ToString(uid); ;
                        //await Notify("UID: " + BitConverter.ToString(uid));

                        ushort maxAddress = 0;
                        switch (cardIdentification.PcscCardName)
                        {
                            case Pcsc.CardName.MifareStandard1K:
                                maxAddress = 0x3f;
                                break;
                            case Pcsc.CardName.MifareStandard4K:
                                maxAddress = 0xff;
                                break;
                        }
                        await mfStdAccess.LoadKeyAsync(MifareStandard.DefaultKeys.FactoryDefault);

                        for (ushort address = 0; address <= maxAddress; address++)
                        {
                            var response = await mfStdAccess.ReadAsync(address, Pcsc.GeneralAuthenticate.GeneralAuthenticateKeyType.MifareKeyA);
                            LogMessage("Block " + address.ToString() + " " + BitConverter.ToString(response));
                        }
                    }
                    else if (cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.StorageClass
                        && (cardIdentification.PcscCardName == Pcsc.CardName.ICODE1 ||
                            cardIdentification.PcscCardName == Pcsc.CardName.ICODESLI ||
                            cardIdentification.PcscCardName == Pcsc.CardName.iCodeSL2))
                    {
                        // Handle ISO15693
                        LogMessage("ISO15693 card detected");
                        var iso15693Access = new Iso15693.AccessHandler(connection);
                        var uid = await iso15693Access.GetUidAsync();
                        UID = BitConverter.ToString(uid);
                        //await Notify("UID: " + BitConverter.ToString(uid));
                    }
                    else
                    {
                        // Unknown card type
                        // Note that when using the XDE emulator the card's ATR and type is not passed through, so we'll
                        // end up here even for known card types if using the XDE emulator

                        // Some cards might still let us query their UID with the PC/SC command, so let's try:
                        var apduRes = await connection.TransceiveAsync(new Pcsc.GetUid());
                        if (!apduRes.Succeeded)
                        {
                            LogMessage("Failure getting UID of card, " + apduRes.ToString());
                        }
                        else
                        {
                            LogMessage("UID:  " + BitConverter.ToString(apduRes.ResponseData));
                            UID = BitConverter.ToString(apduRes.ResponseData);
                        }
                    }

                    // START MANAGING SUP STOCK

                    // UID
                    if (UID != "")
                    {
                        id = ToUID(UID);
                        bool found = false;
                        int i = 0;
                        // check if ID is in left list
                        foreach (var it in AwaySUP)
                        {
                            i++;
                            if (it.IsSameID(id))
                            {
                                found = true;
                                // set price according to dT
                                int Price = it.GetPrice();

                                //wait for validation, then move from left to stock
                                await Notify("! Arrivé, durée = " + it.strTime + "   Prix: " + Price + " CHF");


                                //move from Left to Stock
                                try
                                {
                                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                    () =>
                                    {
                                        // Your UI update code goes here
                                        StockSUP.Add(it);
                                        AwaySUP.Remove(it);

                                        StockShow.UpdateLayout();
                                        LeftShow.UpdateLayout();
                                    }
                                    );
                                }
                                catch (Exception ex)
                                {
                                    await Notify("Error swapping from away to stock" + ex.ToString());
                                }

                            }
                        }
                        i = 0;

                        // check if ID is in stock list
                        if (!found)
                        {
                            foreach (var it in StockSUP)
                            {
                                i++;
                                if (it.IsSameID(id))
                                {
                                    found = true;
                                    // start time
                                    await Notify("! Valide, près à partir !", "Départ");
                                    i = 0;
                                    it.StartTimer();

                                    //move from stock to left list

                                    try
                                    {
                                        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                        () =>
                                        {
                                            // Your UI update code goes here!
                                            AwaySUP.Add(it);
                                            StockSUP.Remove(it);

                                            StockShow.UpdateLayout();
                                            LeftShow.UpdateLayout();
                                        }
                                        );

                                    }
                                    catch (Exception ex)
                                    {
                                        await Notify("Error swapping from stock to away" + ex.ToString());
                                    }

                                }
                            }
                        }
                        // else, adding to stock list

                        if (!found)
                        {

                            if (isNewTagNewSUP)
                            {
                                await Notify("! Nouvelle planche ajoutée.");
                                isNewTagNewSUP = false;
                                try
                                {
                                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                    () =>
                                    {
                                        // Your UI update code goes here!
                                        StockSUP.Add(new SUP(id, SUPCategory[selN]));
                                    }
                                    );
                                }
                                catch (Exception ex)
                                {
                                    await Notify("Error adding to stock" + ex.ToString());
                                }
                                showNbStock.Text = StockSUP.Count.ToString();

                                StockShow.UpdateLayout();
                                LeftShow.UpdateLayout();
                            }
                            else
                            {
                                await Notify("! Cette planche n'est pas répértoriée !");
                            }
                        }

                    }



                }
            }
            catch (Exception ex)
            {
                LogMessage("Exception handling card: " + ex.ToString(), NotifyType.ErrorMessage);
            }

        }



        private async Task Notify(string message, string valid = "OK")
        {
            var msgbox = new Windows.UI.Popups.MessageDialog(message);
            msgbox.Commands.Add(new Windows.UI.Popups.UICommand(valid));
            await msgbox.ShowAsync();
        }



        private void newSUP_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            isNewTagNewSUP = true;
        }

        private void Flyout_Closed(object sender, object e)
        {
            isNewTagNewSUP = false;
        }

        private void btnLFTRefreshTime_Click(object sender, RoutedEventArgs e)
        {
            LeftShow.ItemsSource = null;
            LeftShow.ItemsSource = AwaySUP;

        }

        private void NS_Cat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selN = NS_Cat.SelectedIndex;
        }

        private void TglBlockModifType_Toggled(object sender, RoutedEventArgs e)
        {
            if (TglBlockModifType.IsOn)
            {   // unBlock
                LV_Category.Opacity = 1;
                LV_Category.IsEnabled = true;
            }
            else
            {   // Block
                LV_Category.Opacity = 0.5;
                LV_Category.IsEnabled = false;
            }
        }

        private void TM_N_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TM_D_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TM_H_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TM_S_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            InitNFC();
        }

        private void btn_DelCat_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            for (int i = SUPCategory.Count - 1; i >= 0; i--)
            {
                if (SUPCategory[i].Name == btn.Tag.ToString())
                {
                    SUPCategory.RemoveAt(i);
                    break;
                }
            }
        }

        private void btn_DelSUP_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            for (int i = StockSUP.Count - 1; i >= 0; i--)
            {
                if (StockSUP[i].getID().ToString() == btn.Tag.ToString())
                {
                    StockSUP.RemoveAt(i);
                    break;
                }
            }
        }
    }
}

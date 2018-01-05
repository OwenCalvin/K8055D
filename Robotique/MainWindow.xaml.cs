using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Robotique {
    public partial class MainWindow : Window {
        List<CheckboxV2> lCbxV2 = new List<CheckboxV2>();
        DispatcherTimer timer;
        
        bool bConnected = false;
        bool bOutputsTest = false;
        int iOutputTest = 0;
        int iOutput = 1;
        int iAdresse = 3;   // ADRESSE DE CONNEXION

        public MainWindow() {
            InitializeComponent();

            // LIST DE MES CHECKBOXS PERSONNALISEE
            #region LIST CBXV2
            lCbxV2.Add(new CheckboxV2(brdSK5, ellSK5));
            lCbxV2.Add(new CheckboxV2(brdSK6, ellSK6));

            lCbxV2.Add(new CheckboxV2(brdInput1, ellInput1, 1, "input", false));
            lCbxV2.Add(new CheckboxV2(brdInput2, ellInput2, 2, "input", false));
            lCbxV2.Add(new CheckboxV2(brdInput3, ellInput3, 3, "input", false));
            lCbxV2.Add(new CheckboxV2(brdInput4, ellInput4, 4, "input", false));
            lCbxV2.Add(new CheckboxV2(brdInput5, ellInput5, 5, "input", false));

            lCbxV2.Add(new CheckboxV2(brdOutput1, ellOutput1, 1, "output", false));
            lCbxV2.Add(new CheckboxV2(brdOutput2, ellOutput2, 2, "output", false));
            lCbxV2.Add(new CheckboxV2(brdOutput3, ellOutput3, 3, "output", false));
            lCbxV2.Add(new CheckboxV2(brdOutput4, ellOutput4, 4, "output", false));
            lCbxV2.Add(new CheckboxV2(brdOutput5, ellOutput5, 5, "output", false));
            lCbxV2.Add(new CheckboxV2(brdOutput6, ellOutput6, 6, "output", false));
            lCbxV2.Add(new CheckboxV2(brdOutput7, ellOutput7, 7, "output", false));
            lCbxV2.Add(new CheckboxV2(brdOutput8, ellOutput8, 8, "output", false));
            #endregion

            // AFFICHE LA VERISON DE LA DLL
            int ver = Version();
            lblVersion.Content = "Vesion de la DLL: " + (ver >> 24).ToString() + "." + ((ver >> 16) & 0xFF).ToString() + "."+ ((ver >> 8) & 0xFF).ToString() + "." + (ver & 0xFF).ToString();

            // INITIALISE TIMER
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += timer_Tick;
        }

        private void timer_Tick(object sender, EventArgs e) {
            // TEST DES INPUTS
            if (bOutputsTest) {
                iOutputTest += 1;
                if (iOutputTest >= 3) {
                    ClearAllDigital();
                    foreach (CheckboxV2 cbx in lCbxV2) {
                        if (cbx.GroupName == "output") {
                            if (cbx.ActionNumber == iOutput) cbx.Off = false;
                            else cbx.Off = true;
                        }
                    }
                    SetDigitalChannel(iOutput);
                    iOutput++;
                    iOutputTest = 0;
                    if (iOutput > 8) iOutput = 0;
                }
            }
            
            // MISE A JOUR VALEUR POTENTIOMETRE
            brdAD1.Height = ReadAnalogChannel(1);
            OutputAnalogChannel(2, ReadAnalogChannel(2));

            lblPourcentAD1.Content = Math.Round(ReadAnalogChannel(1) / 255.0 * 100.0) + " %";
            lblPourcentAD2.Content = Math.Round(ReadAnalogChannel(2) / 255.0 * 100.0) + " %";

            OutputAnalogChannel(1, ReadAnalogChannel(1));
            brdAD2.Height = ReadAnalogChannel(2);

            // MISE A JOUR VALEUR INPUT
            foreach (CheckboxV2 cbx in lCbxV2) {
                if (cbx.GroupName == "input") {
                    cbx.Off = !ReadDigitalChannel(cbx.ActionNumber);
                }
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e) {
            ClearErrorMessage();
            if (bConnected) {
                ErrorMessage("Une carte est déjà connectée");
            } else {
                // SI CARTE TROUVEE
                if (SearchDevices() == 1 && OpenDevice(iAdresse) >= 0) {
                    lblDevice.Content = "Carte n° " + iAdresse + " connectée";

                    // PEUX CHANGER VALEUR DES SORTIES
                    foreach (CheckboxV2 cbx in lCbxV2) {
                        if (cbx.GroupName == "output") cbx.CanUserSwitch = true;
                    }

                    bConnected = true;
                    lblTipp.Visibility = Visibility.Visible;
                    timer.Start();
                }
                else {
                    ResetAll();
                    lblDevice.Content = "Carte n° " + iAdresse + " non trouvée";
                    ErrorMessage("Pas de carte trouvée à l'adresse: " + iAdresse);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e) {
            ResetAll();
        }

        private void btnOutputsTest_click(object sender, RoutedEventArgs e) {
            if (bConnected) {
                // COMMENCE LE TEST DES SORTIES
                ClearErrorMessage();
                bOutputsTest = !bOutputsTest;

                if (bOutputsTest) btnOutputsTest.Content = "Arrêter le test";
                else {
                    // REMET A ZERO
                    btnOutputsTest.Content = "Tester les sorties";
                    foreach (CheckboxV2 cbx in lCbxV2) {
                        if (cbx.GroupName == "output") cbx.Off = true;
                    }
                }
            } else {
                ErrorMessage(null);
            }
        }

        private void btnDeco_Click(object sender, RoutedEventArgs e) {
            if (bConnected) {
                // REMET LES VALEUR DU PROGRAMME A ZERO
                ClearErrorMessage();
                ResetAll();
                lblDevice.Content = "Aucune carte connectée";
                btnOutputsTest.Content = "Tester les sorties";
                foreach (CheckboxV2 cbx in lCbxV2) {
                    if (cbx.GroupName == "output") {
                        cbx.Off = true;
                        cbx.CanUserSwitch = false;
                    }
                }
                lblTipp.Visibility = Visibility.Hidden;
                bOutputsTest = false;
            } else {
                ErrorMessage(null);
            }
        }

        private void btnSetDigital_click(object sender, RoutedEventArgs e) {
            if (bConnected) {
                if (!bOutputsTest) {
                    ClearErrorMessage();

                    // REGARDE SI DEJA TOUS EST COCHE
                    int iAll = 0;
                    foreach (CheckboxV2 cbx in lCbxV2) {
                        if (cbx.GroupName == "output") {
                            if (!cbx.Off) iAll++;
                            cbx.Off = false;
                        }
                    }
                    if (iAll >= 8) ErrorMessage("Les sorties sont déjà toutes activées");

                    SetAllDigital();
                } else {
                    ErrorMessage("Le test des sorties est en cours d'exécution");
                }
            } else {
                ErrorMessage(null);
            }
        }

        private void btnClearDigital_click(object sender, RoutedEventArgs e) {
            if (bConnected) {
                if (!bOutputsTest) {
                    ClearErrorMessage();

                    // REGARDE SI DEJA TOUS EST COCHE
                    int iAll = 0;
                    foreach (CheckboxV2 cbx in lCbxV2) {
                        if (cbx.GroupName == "output") {
                            if (cbx.Off) iAll++;
                            cbx.Off = true;
                        }
                    }
                    if (iAll >= 8) ErrorMessage("Les sorties sont déjà toutes désactivées");

                    ClearAllDigital();
                } else {
                    ErrorMessage("Le test des sorties est en cours d'exécution");
                }
            } else {
                ErrorMessage(null);
            }
        }

        private void ErrorMessage(string msg) {
            // MESSAGE D'ERREUR
            brdError.Visibility = Visibility.Visible;
            if (msg == null) lblError.Content = "Vous devez connecter une carte";
            else lblError.Content = msg;
        }

        private void ClearErrorMessage() {
            brdError.Visibility = Visibility.Hidden;
        }
        private void ResetAll() {
            // MET VALEURS A ZERO
            ClearAllAnalog();
            ClearAllDigital();
            CloseDevice();
            bConnected = false;
        }

        private void brdSK_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ClearErrorMessage();

            Border brdSender = (Border)sender;

            foreach (CheckboxV2 cbx in lCbxV2) {
                if (cbx.Border == brdSender) {
                    if (cbx.CanUserSwitch) {
                        // SI GROUPE = OUTPUT, CHANGE LA VALEUR
                        if (cbx.GroupName == "output") {
                            if (cbx.Off) SetDigitalChannel(cbx.ActionNumber);
                            else ClearDigitalChannel(cbx.ActionNumber);
                        }

                        // CHANGE LA VALEUR DU CHECKBOX
                        cbx.Off = !cbx.Off;
                    } else if (!bConnected) {
                        ErrorMessage(null);
                    }
                }

                // CHANGE LES CAVALIERS
                if (lCbxV2[0].Off && lCbxV2[1].Off) iAdresse = 3;
                else if (!lCbxV2[0].Off && lCbxV2[1].Off) iAdresse = 2;
                else if (lCbxV2[0].Off && !lCbxV2[1].Off) iAdresse = 1;
                else if (!lCbxV2[0].Off && !lCbxV2[1].Off) iAdresse = 0;

                lblAdresse.Content = "Adresse n° " + iAdresse;
            }
        }

        #region IMPORTATION
        [DllImport("k8055d.dll")]
        public static extern int OpenDevice(int CardAddress);

        [DllImport("k8055d.dll")]
        public static extern void CloseDevice();

        [DllImport("k8055d.dll")]
        public static extern int ReadAnalogChannel(int Channel);

        [DllImport("k8055d.dll")]
        public static extern void ReadAllAnalog(ref int Data1, ref int Data2);

        [DllImport("k8055d.dll")]
        public static extern void OutputAnalogChannel(int Channel, int Data);

        [DllImport("k8055d.dll")]
        public static extern void OutputAllAnalog(int Data1, int Data2);

        [DllImport("k8055d.dll")]
        public static extern void ClearAnalogChannel(int Channel);

        [DllImport("k8055d.dll")]
        public static extern void SetAllAnalog();

        [DllImport("k8055d.dll")]
        public static extern void ClearAllAnalog();

        [DllImport("k8055d.dll")]
        public static extern void SetAnalogChannel(int Channel);

        [DllImport("k8055d.dll")]
        public static extern void WriteAllDigital(int Data);

        [DllImport("k8055d.dll")]
        public static extern void ClearDigitalChannel(int Channel);

        [DllImport("k8055d.dll")]
        public static extern void ClearAllDigital();

        [DllImport("k8055d.dll")]
        public static extern void SetDigitalChannel(int Channel);

        [DllImport("k8055d.dll")]
        public static extern void SetAllDigital();

        [DllImport("k8055d.dll")]
        public static extern bool ReadDigitalChannel(int Channel);

        [DllImport("k8055d.dll")]
        public static extern int ReadAllDigital();

        [DllImport("k8055d.dll")]
        public static extern int ReadCounter(int CounterNr);

        [DllImport("k8055d.dll")]
        public static extern void ResetCounter(int CounterNr);

        [DllImport("k8055d.dll")]
        public static extern void SetCounterDebounceTime(int CounterNr, int DebounceTime);

        [DllImport("k8055d.dll")]
        public static extern int Version();

        [DllImport("k8055d.dll")]
        public static extern int SearchDevices();

        [DllImport("k8055d.dll")]
        public static extern int SetCurrentDevice(int lngCardAddress);
        #endregion
    }
}

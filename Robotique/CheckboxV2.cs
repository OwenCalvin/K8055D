using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Robotique {
    class CheckboxV2 {

        #region ATTRIBUTS
        private bool bOff = true;
        public bool Off {
            get { return bOff; }
            set {
                bOff = value;
                double i;

                // BOUGE L'ELLIPSE ET CHANGE LE FOND DU BORDER EN FONCTION DE LA VALEUR
                if (Off) {
                    i = 2;
                    Border.Background = new SolidColorBrush(Color.FromRgb(218, 218, 218)); ;
                } else {
                    i = Border.Width - Ellipse.Width - 4;
                    Border.Background = new SolidColorBrush(Color.FromRgb(30, 215, 96));
                }

                Ellipse.Margin = new Thickness(i, Ellipse.Margin.Top, Ellipse.Margin.Right, Ellipse.Margin.Bottom);
            }
        }
        public bool CanUserSwitch { get; set; } = true; // PEUX CHANGER LA VALEUR DU CHECKBOX
        public int ActionNumber { get; set; }           // ID DE L'ACTION A EXECUTER
        public string GroupName { get; set; }           // NOM DU GROUPE DE CHECKBOXV2
        public string Name { get; private set; }        // NOM DU COMPOSANT (COMPARABLE A ID)
        public Ellipse Ellipse { get; private set; }    // COMPOSANT ELLIPSE
        public Border Border { get; private set; }      // COMPOSANT BORDER
        #endregion

        #region CONSTRUCTEURS
        public CheckboxV2(Border Border, Ellipse Ellipse) {
            this.Border = Border;
            this.Ellipse = Ellipse;
        }

        public CheckboxV2(Border Border, Ellipse Ellipse, int ActionNumber, string GroupName, bool CanUserSwitch) {
            this.Border = Border;
            this.Ellipse = Ellipse;
            this.ActionNumber = ActionNumber;
            this.GroupName = GroupName;
            this.CanUserSwitch = CanUserSwitch;
        }
        #endregion

        #region METHODES
        #endregion
    }
}

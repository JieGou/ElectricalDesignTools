using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Validators.Cable;
public class CecCableLengthValidationMessages
{
    public static string Rule14_100_B { get; set; } = "CEC Rule 14-100(b) - Maximum conductor length for smaller tap conductors is 3m." + "\n" +
                                                      "All other items of Rule 14-100(b) must also be considered.";
    public static string Rule14_100_B_Caption { get; set; } = "Cable length violation - CEC Rule 14-100(b).";

    public static string Rule14_100_C { get; set; } = "CEC Rule 14-100(c) - Maximum conductor length for tap conductors where the smaller conductoris 7.5m." + "\n" +
                                                      "All other items of Rule 14-100(b) must also be considered.";
    public static string Rule14_100_C_Caption { get; set; } = "Cable length violation - CEC Rule 14-100(c).";

    public static string Rule14_100_D { get; set; } = "CEC Rule 14-100(d) - Maximum conductor length for equipment fed from transformers rated over 750V without secondary protection is 7.5 meters. " + "\n" +
                                                      "An OCPD must be added to protect the transformer secondary conductor for its length to be more than 7.5 meters." + "\n" +
                                                      "All other items of Rule 14-100(d) must also be considered.";
    public static string Rule14_100_D_Caption { get; set; } = "Cable length violation - CEC Rule 14-100(d).";
}

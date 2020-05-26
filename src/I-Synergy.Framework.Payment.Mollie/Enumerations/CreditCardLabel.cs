﻿using System.Runtime.Serialization;

namespace ISynergy.Framework.Payment.Mollie.Enumerations
{
    /// <summary>
    /// The card's label. Note that not all labels can be acquired through ISynergy.Framework.Payment.Mollie.
    /// </summary>
    public enum CreditCardLabel
    {
        /// <summary>
        /// The american express
        /// </summary>
        /// <autogeneratedoc />
        [EnumMember(Value = "American Express")] AmericanExpress,
        /// <summary>
        /// The carta si
        /// </summary>
        /// <autogeneratedoc />
        [EnumMember(Value = "Carta si")] CartaSi,
        /// <summary>
        /// The carte bleue
        /// </summary>
        /// <autogeneratedoc />
        [EnumMember(Value = "Carte Bleue")] CarteBleue,
        /// <summary>
        /// The dankort
        /// </summary>
        /// <autogeneratedoc />
        Dankort,
        /// <summary>
        /// The diners club
        /// </summary>
        /// <autogeneratedoc />
        [EnumMember(Value = "Diners Club")] DinersClub,
        /// <summary>
        /// The discover
        /// </summary>
        /// <autogeneratedoc />
        Discover,
        /// <summary>
        /// The JCB
        /// </summary>
        /// <autogeneratedoc />
        [EnumMember(Value = "JCB")] Jcb,
        /// <summary>
        /// The laser
        /// </summary>
        /// <autogeneratedoc />
        [EnumMember(Value = "Laser")] Laser,
        /// <summary>
        /// The maestro
        /// </summary>
        /// <autogeneratedoc />
        Maestro,
        /// <summary>
        /// The mastercard
        /// </summary>
        /// <autogeneratedoc />
        Mastercard,
        /// <summary>
        /// The unionpay
        /// </summary>
        /// <autogeneratedoc />
        Unionpay,
        /// <summary>
        /// The visa
        /// </summary>
        /// <autogeneratedoc />
        Visa
    }
}

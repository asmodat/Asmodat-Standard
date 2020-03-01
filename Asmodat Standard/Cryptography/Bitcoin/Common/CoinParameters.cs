// <copyright file="CoinParameters.cs" company="SoftChains">
//  Copyright 2016 Dan Gershony
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
namespace Asmodat.Cryptography.Bitcoin.Common
{
    using Asmodat.Cryptography.Bitcoin.Address;

    /// <summary>
    /// The parameters of Bitcoin networks (each Blockchain has its own values for network parameters).
    /// </summary>
    public class CoinParameters
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the first byte of a base58 encoded address. See <see cref="BitcoinPublicKey"/>
        /// </summary>
        public int PublicKeyAddressVersion { get; set; }

        /// <summary>
        /// Gets or sets the first byte of a base58 encoded dumped key. See <see cref="BitcoinPrivateKey"/>.
        /// </summary>
        public int PrivateKeyVersion { get; set; }

        /// <summary>
        /// Gets or sets the pay to script hash header.
        /// </summary>
        public int ScriptAddressVersion { get; set; }

        /// <summary>
        /// Gets or sets the HD key index.
        /// </summary>
        public int ExtendedKeyIndex { get; set; }

        /// <summary>
        /// Gets or sets the coin tag.
        /// </summary>
        public string CoinTag { get; set; }

        /// <summary>
        /// Gets or sets the scale of conversion between the raw hex coin value and the representation value of money.
        /// </summary>
        public long CoinScale { get; set; }

        /// <summary>
        /// Gets or sets the transaction version.
        /// </summary>
        public int TransactionVersion { get; set; }

        #endregion
    }
}
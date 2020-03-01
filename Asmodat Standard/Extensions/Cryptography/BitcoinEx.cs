﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Types;
using System.Linq;
using Asmodat.Cryptography.Bitcoin.Mnemonic;

namespace AsmodatStandard.Extensions.Cryptography
{
    public static class BitcoinEx
    {
        public static string RandomBip39Mnemonic(int length)
        {
            if (length < 12)
                throw new ArgumentException($"{nameof(length)} can't be less then 12");

            var el = (length - 12) / 6;
            el = 16 + (el * 8);
            var entropy = RandomEx.NextBytesSecure(el);
            var bip39 = new Bip39(entropyBytes: entropy, passphrase: "", language: Bip39.Language.English);
            var mnemonic = bip39.MnemonicSentence;
            var ml = mnemonic.Split(' ').Length;
            if (ml != length)
                throw new Exception($"Generated mnemonic length is {ml}, expected {length}");

            return mnemonic;
        }

            /// <summary>
            /// https://github.com/satoshilabs/slips/blob/master/slip-0044.md
            /// </summary>
            /// <param name="coin"></param>
            /// <returns></returns>
        public static int GetCoinIndex(string coin)
        {
            if (coin.IsNullOrWhitespace())
                return -1;

            var lines = BIP44MDTABLE.Split('\n');

            coin = $" | {coin.ToUpper()} ";
            foreach(var line in lines)
            {
                if (line.IsNullOrEmpty())
                    continue;

                if (!line.ToUpper().Contains(coin))
                    continue;

                var index = line.Split('|').FirstOrDefault().ToIntOrDefault(-1);
                if (index >= 0)
                    return index;
            }

            return -1;
        }

        private static readonly string BIP44MDTABLE =
@"
0     | 0x80000000 | BTC    | [Bitcoin](https://bitcoin.org/)
1     | 0x80000001 |        | Testnet (all coins)
2     | 0x80000002 | LTC    | [Litecoin](https://litecoin.org/)
3     | 0x80000003 | DOGE   | [Dogecoin](https://github.com/dogecoin/dogecoin)
4     | 0x80000004 | RDD    | Reddcoin
5     | 0x80000005 | DASH   | [Dash](https://github.com/dashpay/dash) (ex Darkcoin)
6     | 0x80000006 | PPC    | [Peercoin](https://peercoin.net/)
7     | 0x80000007 | NMC    | [Namecoin](http://namecoin.info/)
8     | 0x80000008 | FTC    | [Feathercoin](https://www.feathercoin.com/)
9     | 0x80000009 | XCP    | [Counterparty](http://counterparty.io/)
10    | 0x8000000a | BLK    | [Blackcoin](http://blackcoin.co/)
11    | 0x8000000b | NSR    | [NuShares](https://nubits.com/nushares/introduction)
12    | 0x8000000c | NBT    | NuBits
13    | 0x8000000d | MZC    | Mazacoin
14    | 0x8000000e | VIA    | Viacoin
15    | 0x8000000f | XCH    | ClearingHouse
16    | 0x80000010 | RBY    | Rubycoin
17    | 0x80000011 | GRS    | Groestlcoin
18    | 0x80000012 | DGC    | Digitalcoin
19    | 0x80000013 | CCN    | Cannacoin
20    | 0x80000014 | DGB    | DigiByte
21    | 0x80000015 |        | [Open Assets](https://github.com/OpenAssets/open-assets-protocol)
22    | 0x80000016 | MONA   | Monacoin
23    | 0x80000017 | CLAM   | Clams
24    | 0x80000018 | XPM    | Primecoin
25    | 0x80000019 | NEOS   | Neoscoin
26    | 0x8000001a | JBS    | Jumbucks
27    | 0x8000001b | ZRC    | ziftrCOIN
28    | 0x8000001c | VTC    | Vertcoin
29    | 0x8000001d | NXT    | NXT
30    | 0x8000001e | BURST  | Burst
31    | 0x8000001f | MUE    | MonetaryUnit
32    | 0x80000020 | ZOOM   | Zoom
33    | 0x80000021 | VASH   | [Virtual Cash](http://www.bitnet.cc/) Also known as VPNcoin
34    | 0x80000022 | CDN    | [Canada eCoin](https://github.com/Canada-eCoin/)
35    | 0x80000023 | SDC    | ShadowCash
36    | 0x80000024 | PKB    | [ParkByte](https://github.com/parkbyte/)
37    | 0x80000025 | PND    | Pandacoin
38    | 0x80000026 | START  | StartCOIN
39    | 0x80000027 | MOIN   | [MOIN](https://discovermoin.com)
40    | 0x80000028 | EXP    | [Expanse](http://www.expanse.tech/)
41    | 0x80000029 | EMC2   | [Einsteinium](https://www.emc2.foundation/)
42    | 0x8000002a | DCR    | [Decred](https://decred.org/)
43    | 0x8000002b | XEM    | [NEM](https://github.com/NemProject)
44    | 0x8000002c | PART   | [Particl](https://particl.io/)
45    | 0x8000002d | ARG    | [Argentum](http://www.argentum.io)
46    | 0x8000002e |        | [Libertas](https://github.com/dangershony/Libertas)
47    | 0x8000002f |        | [Posw coin](https://poswallet.com)
48    | 0x80000030 | SHR    | [Shreeji](https://github.com/SMJBIT/SHREEJI)
49    | 0x80000031 | GCR    | Global Currency Reserve (GCRcoin)
50    | 0x80000032 | NVC    | [Novacoin](https://github.com/novacoin-project/novacoin)
51    | 0x80000033 | AC     | [Asiacoin](https://github.com/AsiaCoin/AsiaCoinFix)
52    | 0x80000034 | BTCD   | [Bitcoindark](https://github.com/jl777/btcd)
53    | 0x80000035 | DOPE   | [Dopecoin](https://github.com/dopecoin-dev/DopeCoinV3)
54    | 0x80000036 | TPC    | [Templecoin](https://github.com/9cat/templecoin)
55    | 0x80000037 | AIB    | [AIB](https://github.com/iobond/aib)
56    | 0x80000038 | EDRC   | [EDRCoin](https://github.com/EDRCoin/EDRcoin-src)
57    | 0x80000039 | SYS    | [Syscoin](https://github.com/syscoin/syscoin2)
58    | 0x8000003a | SLR    | [Solarcoin](https://github.com/onsightit/solarcoin)
59    | 0x8000003b | SMLY   | [Smileycoin](https://github.com/tutor-web/smileyCoin)
60    | 0x8000003c | ETH    | [Ether](https://ethereum.org/ether)
61    | 0x8000003d | ETC    | [Ether Classic](https://ethereumclassic.github.io)
62    | 0x8000003e | PSB    | [Pesobit](https://github.com/pesobitph/pesobit-source)
63    | 0x8000003f | LDCN   | [Landcoin](http://landcoin.co/)
64    | 0x80000040 |        | [Open Chain](https://github.com/openchain/)
65    | 0x80000041 | XBC    | [Bitcoinplus](https://bitcoinplus.org)
66    | 0x80000042 | IOP    | [Internet of People](http://www.fermat.org)
67    | 0x80000043 | NXS    | [Nexus](http://www.nexusearth.com/)
68    | 0x80000044 | INSN   | [InsaneCoin](http://insanecoin.com)
69    | 0x80000045 | OK     | [OKCash](https://github.com/okcashpro/)
70    | 0x80000046 | BRIT   | [BritCoin](https://britcoin.com)
71    | 0x80000047 | CMP    | [Compcoin](https://compcoin.com)
72    | 0x80000048 | CRW    | [Crown](http://crown.tech/)
73    | 0x80000049 | BELA   | [BelaCoin](http://belacoin.org)
74    | 0x8000004a | ICX    | [ICON](https://icon.foundation/)
75    | 0x8000004b | FJC    | [FujiCoin](http://www.fujicoin.org/)
76    | 0x8000004c | MIX    | [MIX](https://www.mix-blockchain.org/)
77    | 0x8000004d | XVG    | [Verge](https://github.com/vergecurrency/verge/)
78    | 0x8000004e | EFL    | [Electronic Gulden](https://egulden.org/)
79    | 0x8000004f | CLUB   | [ClubCoin](https://clubcoin.co/)
80    | 0x80000050 | RICHX  | [RichCoin](https://richcoin.us/)
81    | 0x80000051 | POT    | [Potcoin](http://potcoin.com/)
82    | 0x80000052 | QRK    | Quarkcoin
83    | 0x80000053 | TRC    | [Terracoin](https://terracoin.io/)
84    | 0x80000054 | GRC    | Gridcoin
85    | 0x80000055 | AUR    | [Auroracoin](http://auroracoin.is/)
86    | 0x80000056 | IXC    | IXCoin
87    | 0x80000057 | NLG    | [Gulden](https://Gulden.com/)
88    | 0x80000058 | BITB   | [BitBean](http://bitbean.org/)
89    | 0x80000059 | BTA    | [Bata](http://bata.io/)
90    | 0x8000005a | XMY    | [Myriadcoin](http://myriadcoin.org)
91    | 0x8000005b | BSD    | [BitSend](http://bitsend.info)
92    | 0x8000005c | UNO    | [Unobtanium](http://http://unobtanium.uno/)
93    | 0x8000005d | MTR    | [MasterTrader](https://github.com/CrypticApplications/MTR-Update/)
94    | 0x8000005e | GB     | [GoldBlocks](https://github.com/goldblockscoin/goldblocks)
95    | 0x8000005f | SHM    | [Saham](https://github.com/SahamDev/SahamDev)
96    | 0x80000060 | CRX    | [Chronos](https://github.com/chronoscoin/Chronoscoin)
97    | 0x80000061 | BIQ    | [Ubiquoin](https://github.com/ubiquoin/ubiq)
98    | 0x80000062 | EVO    | [Evotion](https://github.com/evoshiun/Evotion)
99    | 0x80000063 | STO    | [SaveTheOcean](https://github.com/SaveTheOceanMovement/SaveTheOceanCoin)
100   | 0x80000064 | BIGUP  | [BigUp](https://github.com/BigUps/)
101   | 0x80000065 | GAME   | [GameCredits](https://github.com/gamecredits-project)
102   | 0x80000066 | DLC    | [Dollarcoins](https://github.com/dollarcoins/source)
103   | 0x80000067 | ZYD    | [Zayedcoin](https://github.com/ZayedCoin/Zayedcoin)
104   | 0x80000068 | DBIC   | [Dubaicoin](https://github.com/DubaiCoinDev/DubaiCoin)
105   | 0x80000069 | STRAT  | [Stratis](http://www.stratisplatform.com)
106   | 0x8000006a | SH     | [Shilling](https://github.com/yavwa/Shilling)
107   | 0x8000006b | MARS   | [MarsCoin](http://www.marscoin.org/)
108   | 0x8000006c | UBQ    | [Ubiq](https://github.com/Ubiq)
109   | 0x8000006d | PTC    | [Pesetacoin](http://pesetacoin.info/)
110   | 0x8000006e | NRO    | [Neurocoin](https://neurocoin.org)
111   | 0x8000006f | ARK    | [ARK](https://ark.io)
112   | 0x80000070 | USC    | [UltimateSecureCashMain](http://ultimatesecurecash.info)
113   | 0x80000071 | THC    | [Hempcoin](http://hempcoin.org)
114   | 0x80000072 | LINX   | [Linx](https://mylinx.io)
115   | 0x80000073 | ECN    | [Ecoin](https://www.ecoinsource.com)
116   | 0x80000074 | DNR    | [Denarius](https://denarius.io)
117   | 0x80000075 | PINK   | [Pinkcoin](http://getstarted.with.pink)
118   | 0x80000076 | ATOM   | [Atom](https://cosmos.network)
119   | 0x80000077 | PIVX   | [Pivx](https://github.com/PIVX-Project/PIVX)
120   | 0x80000078 | FLASH  | [Flashcoin](https://flashcoin.io)
121   | 0x80000079 | ZEN    | [Zencash](https://zensystem.io)
122   | 0x8000007a | PUT    | [Putincoin](https://putincoin.info)
123   | 0x8000007b | ZNY    | [BitZeny](https://bitzeny.tech/)
124   | 0x8000007c | UNIFY  | [Unify](http://unifycryptocurrency.com)
125   | 0x8000007d | XST    | [StealthCoin](http://www.stealthcoin.com)
126   | 0x8000007e | BRK    | [Breakout Coin](http://www.breakoutcoin.com)
127   | 0x8000007f | VC     | [Vcash](https://vcash.info)
128   | 0x80000080 | XMR    | [Monero](https://getmonero.org/)
129   | 0x80000081 | VOX    | [Voxels](https://www.voxelus.com)
130   | 0x80000082 | NAV    | [NavCoin](https://github.com/navcoindev/navcoin2)
131   | 0x80000083 | FCT    | [Factom Factoids](https://github.com/FactomProject/FactomDocs/blob/master/wallet_info/wallet_test_vectors.md)
132   | 0x80000084 | EC     | [Factom Entry Credits](https://github.com/FactomProject)
133   | 0x80000085 | ZEC    | [Zcash](https://z.cash)
134   | 0x80000086 | LSK    | [Lisk](https://lisk.io/)
135   | 0x80000087 | STEEM  | [Steem](http://steem.io)
136   | 0x80000088 | XZC    | [ZCoin](https://zcoin.io)
137   | 0x80000089 | RBTC   | [RSK](http://www.rsk.co/)
138   | 0x8000008a |        | [Giftblock](https://github.com/gyft/giftblock)
139   | 0x8000008b | RPT    | [RealPointCoin](https://github.com/MaxSmile/RealPointCoinQt)
140   | 0x8000008c | LBC    | [LBRY Credits](https://lbry.io/)
141   | 0x8000008d | KMD    | [Komodo](https://komodoplatform.com/)
142   | 0x8000008e | BSQ    | [bisq Token](http://bisq.io/)
143   | 0x8000008f | RIC    | [Riecoin](https://github.com/riecoin/riecoin)
144   | 0x80000090 | XRP    | [Ripple](https://ripple.com)
145   | 0x80000091 | BCH    | [Bitcoin Cash](https://www.bitcoincash.org)
146   | 0x80000092 | NEBL   | [Neblio](https://nebl.io)
147   | 0x80000093 | ZCL    | [ZClassic](http://zclassic.org/)
148   | 0x80000094 | XLM    | [Stellar Lumens](https://www.stellar.org/)
149   | 0x80000095 | NLC2   | [NoLimitCoin2](http://www.nolimitcoin.org)
150   | 0x80000096 | WHL    | [WhaleCoin](https://whalecoin.org/)
151   | 0x80000097 | ERC    | [EuropeCoin](https://www.europecoin.eu.org/)
152   | 0x80000098 | DMD    | [Diamond](http://bit.diamonds)
153   | 0x80000099 | BTM    | [Bytom](https://bytom.io)
154   | 0x8000009a | BIO    | [Biocoin](https://biocoin.bio)
155   | 0x8000009b | XWC    | [Whitecoin](https://www.whitecoin.info)
156   | 0x8000009c | BTG    | [Bitcoin Gold](http://www.btcgpu.org)
157   | 0x8000009d | BTC2X  | [Bitcoin 2x](https://medium.com/@DCGco/bitcoin-scaling-agreement-at-consensus-2017-133521fe9a77)
158   | 0x8000009e | SSN    | [SuperSkynet](http://wwww.superskynet.org/)
159   | 0x8000009f | TOA    | [TOACoin](http://www.toacoin.com)
160   | 0x800000a0 | BTX    | [Bitcore](https://bitcore.cc)
161   | 0x800000a1 | ACC    | [Adcoin](https://www.getadcoin.com/)
162   | 0x800000a2 | BCO    | [Bridgecoin](https://bridgecoin.org/)
163   | 0x800000a3 | ELLA   | [Ellaism](https://ellaism.org)
164   | 0x800000a4 | PIRL   | [Pirl](https://pirl.io)
165   | 0x800000a5 | XRB    | [RaiBlocks](https://raiblocks.com)
166   | 0x800000a6 | VIVO   | [Vivo](https://www.vivocrypto.com/)
167   | 0x800000a7 | FRST   | [Firstcoin](http://firstcoinproject.com)
168   | 0x800000a8 | HNC    | [Helleniccoin](http://www.helleniccoin.gr/)
169   | 0x800000a9 | BUZZ   | [BUZZ](http://www.buzzcoin.info/)
170   | 0x800000aa | MBRS   | [Ember](https://www.embercoin.io/)
171   | 0x800000ab | HSR    | [Hcash](https://h.cash)
172   | 0x800000ac | HTML   | [HTMLCOIN](https://htmlcoin.com/)
173   | 0x800000ad | ODN    | [Obsidian](https://obsidianplatform.com/)
174   | 0x800000ae | ONX    | [OnixCoin](https://www.onixcoin.com/)
175   | 0x800000af | RVN    | [Ravencoin](https://ravencoin.org/)
176   | 0x800000b0 | GBX    | [GoByte](https://gobyte.network)
177   | 0x800000b1 | BTCZ   | [BitcoinZ](https://btcz.rocks/en/)
178   | 0x800000b2 | POA    | [Poa](https://poa.network)
179   | 0x800000b3 | NYC    | [NewYorkCoin](http://nycoin.net)
180   | 0x800000b4 | MXT    | [MarteXcoin](http://martexcoin.org)
181   | 0x800000b5 | WC     | [Wincoin](https://wincoin.co)
182   | 0x800000b6 | MNX    | [Minexcoin](https://minexcoin.com)
183   | 0x800000b7 | BTCP   | [Bitcoin Private](https://btcprivate.org)
184   | 0x800000b8 | MUSIC  | [Musicoin](https://www.musicoin.org)
185   | 0x800000b9 | BCA    | [Bitcoin Atom](https://bitcoinatom.io)
186   | 0x800000ba | CRAVE  | [Crave](https://craveproject.net)
187   | 0x800000bb | STAK   | [STRAKS](https://straks.io)
188   | 0x800000bc | WBTC   | [World Bitcoin](http://www.wbtcteam.org/)
189   | 0x800000bd | LCH    | [LiteCash](http://www.litecash.info/)
190   | 0x800000be | EXCL   | [ExclusiveCoin](https://exclusivecoin.pw/)
191   | 0x800000bf |        | [Lynx](https://getlynx.io)
192   | 0x800000c0 | LCC    | [LitecoinCash](https://litecoinca.sh)
193   | 0x800000c1 | XFE    | [Feirm](https://www.feirm.com)
194   | 0x800000c2 | EOS    | [EOS](https://eos.io)
195   | 0x800000c3 | TRX    | [Tron](https://tron.network/enindex.html)
196   | 0x800000c4 | KOBO   | [Kobocoin](https://kobocoin.com)
197   | 0x800000c5 | HUSH   | [HUSH](https://myhush.org)
198   | 0x800000c6 | BANANO | [Bananos](https://banano.co.in)
199   | 0x800000c7 | ETF    | [ETF](http://ethereumfog.org)
200   | 0x800000c8 | OMNI   | [Omni](http://www.omnilayer.org)
201   | 0x800000c9 | BIFI   | [BitcoinFile](https://www.bitcoinfile.org)
202   | 0x800000ca | UFO    | [Uniform Fiscal Object](https://ufobject.com)
203   | 0x800000cb | CNMC   | [Cryptonodes](https://www.cryptonodes.ch)
204   | 0x800000cc | BCN    | [Bytecoin](http://bytecoin.org)
205   | 0x800000cd | RIN    | [Ringo](http://dkwzjw.github.io/ringo/)
206   | 0x800000ce | ATP    | [PlatON](https://www.platon.network)
207   | 0x800000cf | EVT    | [everiToken](https://everiToken.io)
208   | 0x800000d0 | ATN    | [ATN](https://atn.io)
209   | 0x800000d1 | BIS    | [Bismuth](http://www.bismuth.cz)
210   | 0x800000d2 | NEET   | [NEETCOIN](https://neetcoin.jp/)
211   | 0x800000d3 | BOPO   | [BopoChain](http://www.bopochain.org/)
212   | 0x800000d4 | OOT    | [Utrum](https://utrum.io/ootcoin/)
213   | 0x800000d5 | XSPEC  | [Spectrecoin](https://spectreproject.io/)
214   | 0x800000d5 | MONK   | [Monkey Project](https://www.monkey.vision)
215   | 0x800000d7 | BOXY   | [BoxyCoin](http://www.boxycoin.org/)
216   | 0x800000d8 | FLO    | [Flo](https://www.flo.cash/)
217   | 0x800000d9 | MEC    | [Megacoin](https://www.megacoin.eu)
218   | 0x800000da | BTDX   | [BitCloud](https://bit-cloud.info)
219   | 0x800000db | XAX    | [Artax](https://www.artaxcoin.org/)
220   | 0x800000dc | ANON   | [ANON](https://www.anonfork.io/)
221   | 0x800000dd | LTZ    | [LitecoinZ](https://litecoinz.org/)
222   | 0x800000de | BITG   | [Bitcoin Green](https://savebitcoin.io)
223   | 0x800000df | ASK    | [AskCoin](https://askcoin.org)
224   | 0x800000e0 | SMART  | [Smartcash](https://smartcash.cc)
225   | 0x800000e1 | XUEZ   | [XUEZ](https://xuezcoin.com)
226   | 0x800000e2 | HLM    | [Helium](https://www.heliumlabs.org/)
227   | 0x800000e3 | WEB    | [Webchain](https://webchain.network/)
228   | 0x800000e4 | ACM    | [Actinium](https://actinium.org)
229   | 0x800000e5 | NOS    | [NOS Stable Coins](https://nos.cash)
230   | 0x800000e6 | BITC   | [BitCash](https://www.choosebitcash.com)
231   | 0x800000e7 | HTH    | [Help The Homeless Coin](https://hthcoin.world)
232   | 0x800000e8 | TZC    | [Trezarcoin](https://trezarcoin.com)
233   | 0x800000e9 | VAR    | [Varda](https://varda.io)
234   | 0x800000ea | IOV    | [IOV](https://www.iov.one)
235   | 0x800000eb | FIO    | [FIO](https://fio.foundation)
236   | 0x800000ec | BSV    | [BitcoinSV](https://bitcoinsv.io)
237   | 0x800000ed | DXN    | [DEXON](https://dexon.org/)
238   | 0x800000ee | QRL    | [Quantum Resistant Ledger](https://www.theqrl.org/)
239   | 0x800000ef | PCX    | [ChainX](https://github.com/chainx-org/ChainX)
240   | 0x800000f0 | LOKI   | [Loki](https://github.com/loki-project/loki)
241   | 0x800000f1 |        | [Imagewallet](https://imagewallet.io)
242   | 0x800000f2 | NIM    | [Nimiq](https://nimiq.com/)
243   | 0x800000f3 | SOV    | [Sovereign Coin](http://www.sovcore.org/)
244   | 0x800000f4 | JCT    | [Jibital Coin](https://jibital.ir/)
245   | 0x800000f5 | SLP    | [Simple Ledger Protocol](https://simpleledger.cash)
246   | 0x800000f6 | EWT    | [Energy Web](https://energyweb.org)
247   | 0x800000f7 | UC     | [Ulord](http://ulord.one)
248   | 0x800000f8 | EXOS   | [EXOS](https://economy.openexo.com)
249   | 0x800000f9 | ECA    | [Electra](https://www.electraproject.org)
250   | 0x800000fa | SOOM   | [Soom](http://www.fourthblockchain.org/)
251   | 0x800000fb | XRD    | [Redstone](https://www.redstoneplatform.com/)
252   | 0x800000fc | FREE   | [FreeCoin](https://web.freepay.biz)
253   | 0x800000fd | NPW    | [NewPowerCoin](https://npw.live)
254   | 0x800000fe | BST    | [BlockStamp](https://blockstamp.info)
255   | 0x800000ff |        | [SmartHoldem](https://smartholdem.io)
256   | 0x80000100 | NANO   | [Bitcoin Nano](https://www.btcnano.org)
257   | 0x80000101 | BTCC   | [Bitcoin Core](https://thebitcoincore.org)
258   | 0x80000102 |        | [Zen Protocol](https://www.zenprotocol.com/)
259   | 0x80000103 | ZEST   | [Zest](https://www.zestcoin.io)
260   | 0x80000104 | ABT    | [ArcBlock](https://arcblock.io)
261   | 0x80000105 | PION   | [Pion](https://pioncoin.org/)
262   | 0x80000106 | DT3    | [DreamTeam3](https://crypto-dreamteam.com)
263   | 0x80000107 | ZBUX   | [Zbux](https://z-bux.org)
264   | 0x80000108 | KPL    | [Kepler](https://kepler.cash)
265   | 0x80000109 | TPAY   | [TokenPay](https://tokenpay.com)
266   | 0x8000010a | ZILLA  | [ChainZilla](https://www.chainzilla.io)
267   | 0x8000010b | ANK    | [Anker](https://ankerid.com)
268   | 0x8000010c | BCC    | [BCChain](https://github.com/bc-chain/BCC)
269   | 0x8000010d | HPB    | [HPB](https://hpb.io)
270   | 0x8000010e | ONE    | [ONE](http://www.onechain.one/)
271   | 0x8000010f | SBC    | [SBC](http://www.smartbitcoin.one)
272   | 0x80000110 | IPC    | [IPChain](https://www.ipcchain.org)
273   | 0x80000111 | DMTC   | [Dominantchain](https://dominantchain.com/)
274   | 0x80000112 | OGC    | [Onegram](https://onegram.org/)
275   | 0x80000113 | SHIT   | [Shitcoin](https://shitcoin.org)
276   | 0x80000114 | ANDES  | [Andescoin](https://andes-coin.com)
277   | 0x80000115 | AREPA  | [Arepacoin](https://arepacoinve.info)
278   | 0x80000116 | BOLI   | [Bolivarcoin](https://bolis.info)
279   | 0x80000117 | RIL    | [Rilcoin](https://www.rilcoincrypto.org)
280   | 0x80000118 | HTR    | [Hathor Network](https://hathor.network/)
281   | 0x80000119 | FCTID  | [Factom ID](https://github.com/FactomProject)
282   | 0x8000011a | BRAVO  | [BRAVO](https://bravocoin.com/)
283   | 0x8000011b | ALGO   | [Algorand](https://www.algorand.com/)
284   | 0x8000011c | BZX    | [Bitcoinzero](https://bitcoinzerox.net)
285   | 0x8000011d | GXX    | [GravityCoin](https://www.gravitycoin.io/)
286   | 0x8000011e | HEAT   | [HEAT](https://heatledger.com/)
287   | 0x8000011f | XDN    | [DigitalNote](https://digitalnote.biz)
288   | 0x80000120 | FSN    | [FUSION](https://www.fusion.org/)
289   | 0x80000121 | CPC    | [Capricoin](https://capricoin.org)
290   | 0x80000122 | BOLD   | [Bold](https://boldprivate.network)
291   | 0x80000123 | IOST   | [IOST](https://iost.io/)
292   | 0x80000124 | TKEY   | [Tkeycoin](https://tkeycoin.com)
293   | 0x80000125 | USE    | [Usechain](https://usechain.net)
294   | 0x80000126 | BCZ    | [BitcoinCZ](https://www.bitcoincz.org/)
295   | 0x80000127 | IOC    | [Iocoin](https://iocoin.io)
296   | 0x80000128 | ASF    | [Asofe](https://github.com/TheLightSide/asofe)
297   | 0x80000129 | MASS   | [MASS](https://www.massnet.org)
298   | 0x8000012a | FAIR   | [FairCoin](https://faircoin.world/)
299   | 0x8000012b | NUKO   | [Nekonium](https://nekonium.github.io/)
300   | 0x8000012c | GNX    | [Genaro Network](https://genaro.network/)
301   | 0x8000012d | DIVI   | [Divi Project](https://diviproject.org)
302   | 0x8000012e | CMT    | [Community](https://thecriptocommunity.com)
303   | 0x8000012f | EUNO   | [EUNO](https://euno.co/)
304   | 0x80000130 | IOTX   | [IoTeX](https://iotex.io/)
305   | 0x80000131 | ONION  | [DeepOnion](https://deeponion.org)
306   | 0x80000132 | 8BIT   | [8Bit](https://8bit.cash)
307   | 0x80000133 | ATC    | [AToken Coin](https://www.atoken.com/)
308   | 0x80000134 | BTS    | [Bitshares](https://bitshares.org/)
309   | 0x80000135 | CKB    | [Nervos CKB](https://www.nervos.org)
310   | 0x80000136 | UGAS   | [Ultrain](https://www.ultrain.io/)
311   | 0x80000137 | ADS    | [Adshares](https://adshares.net/)
312   | 0x80000138 | ARA    | [Aura](https://auraledger.com/)
313   | 0x80000139 | ZIL    | [Zilliqa](https://zilliqa.com/)
314   | 0x8000013a | MOAC   | [MOAC](https://moac.io/)
315   | 0x8000013b | SWTC   | [SWTC](http://swtc.top/)
316   | 0x8000013c | VNSC   | [vnscoin](http://www.vnscoin.org/)
317   | 0x8000013d | PLUG   | [Pl^g](https://www.poweredbyplug.com/)
318   | 0x8000013e | MAN    | [Matrix AI Network](https://www.matrix.io/)
319   | 0x8000013f | ECC    | [ECCoin](https://ecc.network)
320   | 0x80000140 | RPD    | [Rapids](https://www.rapidsnetwork.io/)
321   | 0x80000141 | RAP    | [Rapture](https://our-rapture.com/)
322   | 0x80000142 | GARD   | [Hashgard](https://www.hashgard.io/)
323   | 0x80000143 | ZER    | [Zero](https://www.zerocurrency.io/)
324   | 0x80000144 | EBST   | [eBoost](https://eboost.fun/)
325   | 0x80000145 | SHARD  | [Shard](https://shardcoin.io/)
326   | 0x80000146 | LINDA  | [Linda Coin](https://lindacoin.com/)
327   | 0x80000147 | CMM    | [Commercium](https://www.commercium.net/)
328   | 0x80000148 | BLOCK  | [Blocknet](https://blocknet.co/)
329   | 0x80000149 | AUDAX  | [AUDAX](https://www.audaxproject.io)
330   | 0x8000014a | LUNA   | [Terra](https://terra.money)
331   | 0x8000014b | ZPM    | [zPrime](https://github.com/zprimecoin/zprime)
332   | 0x8000014c | KUVA   | [Kuva Utility Note](https://www.kuvacash.com)
333   | 0x8000014d | MEM    | [MemCoin](https://memcoin.org)
334   | 0x8000014e | CS     | [Credits](https://credits.com)
335   | 0x8000014f | SWIFT  | [SwiftCash](https://swiftcash.cc)
336   | 0x80000150 | FIX    | [FIX](https://fix.network)
337   | 0x80000151 | CPC    | [CPChain](https://cpchain.io)
338   | 0x80000152 | VGO    | [VirtualGoodsToken](http://vgo.life)
339   | 0x80000153 | DVT    | [DeVault](https://devault.cc)
340   | 0x80000154 | N8V    | [N8VCoin](https://n8vcoin.io)
341   | 0x80000155 | MTNS   | [OmotenashiCoin](http://omotenashicoin.site/)
342   | 0x80000156 | BLAST  | [BLAST](https://blastblastblast.com/)
343   | 0x80000157 | DCT    | [DECENT](https://decent.ch)
344   | 0x80000158 | AUX    | [Auxilium](https://auxilium.global)
345   | 0x80000159 | USDP   | [USDP](http://www.usdp.pro/)
346   | 0x8000015a | HTDF   | [HTDF](https://www.orientwalt.com/)
347   | 0x8000015b | YEC    | [Ycash](https://www.ycash.xyz/)
348   | 0x8000015c | QLC    | [QLC Chain](https://qlcchain.org)
349   | 0x8000015d | TEA    | [Icetea Blockchain](https://icetea.io/)
350   | 0x8000015e | ARW    | [ArrowChain](https://www.arrowchain.io/)
351   | 0x8000015f | MDM    | [Medium](https://www.themedium.io/)
352   | 0x80000160 | CYB    | [Cybex](https://dex.cybex.io/)
353   | 0x80000161 | LTO    | [LTO Network](https://lto.network)
354   | 0x80000162 | DOT    | [Polkadot](https://polkadot.network/)
355   | 0x80000163 | AEON   | [Aeon](https://www.aeon.cash/)
356   | 0x80000164 | RES    | [Resistance](https://www.resistance.io)
357   | 0x80000165 | AYA    | [Aryacoin](https://aryacoin.io/)
358   | 0x80000166 | DAPS   | [Dapscoin](https://officialdapscoin.com)
359   | 0x80000167 | CSC    | [CasinoCoin](https://casinocoin.org)
360   | 0x80000168 | VSYS   | [V Systems](https://www.v.systems/)
361   | 0x80000169 | NOLLAR | [Nollar](https://nollar.org)
362   | 0x8000016a | XNOS   | [NOS](https://nos.cash)
363   | 0x8000016b | CPU    | [CPUchain](https://cpuchain.org)
364   | 0x8000016c | LAMB   | [Lambda Storage Chain](https://lambda.im)
365   | 0x8000016d | VCT    | [ValueCyber](https://valuecyber.org)
366   | 0x8000016e | CZR    | [Canonchain](http://www.canonchain.com/)
367   | 0x8000016f | ABBC   | [ABBC](https://www.abbcfoundation.com/)
368   | 0x80000170 | HET    | [HET](http://www.hetcoin.info/)
369   | 0x80000171 | XAS    | [Asch](https://asch.io)
370   | 0x80000172 | VDL    | [Vidulum](https://vidulum.app)
371   | 0x80000173 | MED    | [MediBloc](https://medibloc.org)
372   | 0x80000174 | ZVC    | [ZVChain](https://www.zvchain.io)
373   | 0x80000175 | VESTX  | [Vestx](https://www.vestxcoin.com)
374   | 0x80000176 | DBT    | [DarkBit](https://www.DarkBitPay.com)
375   | 0x80000177 | SEOS   | [SuperEOS](https://github.com/supereos)
376   | 0x80000178 | MXW    | [Maxonrow](https://mxw.one/)
377   | 0x80000179 | ZNZ    | [ZENZO](https://zenzo.io/)
378   | 0x8000017a | XCX    | [XChain](https://github.com/xchainxchain)
379   | 0x8000017b | SOX    | [SonicX](https://sonicx.org/)
380   | 0x8000017c | NYZO   | [Nyzo](https://nyzo.co/)
381   | 0x8000017d | ULC    | [ULCoin](http://www.ulwallet.io)
382   | 0x8000017e | RYO    | [Ryo Currency](https://ryo-currency.com/)
383   | 0x8000017f | KAL    | [Kaleidochain](https://kaleidochain.io/)
384   | 0x80000180 | XSN    | [Stakenet](https://xsncoin.io/)
385   | 0x80000181 | DOGEC  | [DogeCash](https://dogec.io/)
386   | 0x80000182 | BMV    | [Bitcoin Matteo's Vision](https://btcmv.org/)
387   | 0x80000183 | QBC    | [Quebecoin](https://github.com/QuebecoinQBC/quebecoin/)
388   | 0x80000184 | IMG    | [ImageCoin](https://imagecoin.imagehosty.com/)
389   | 0x80000185 | QOS    | [QOS](https://github.com/QOSGroup/qos)
390   | 0x80000186 | PKT    | [PKT](https://github.com/pkt-cash/pktd)
391   | 0x80000187 | LHD    | [LitecoinHD](https://ltchd.io)
392   | 0x80000188 | CENNZ  | [CENNZnet](https://centrality.ai)
393   | 0x80000189 | HSN    | [Hyper Speed Network](https://www.hsn.link/)
394   | 0x8000018a | CRO    | [Crypto.com Chain](https://github.com/crypto-com/chain)
395   | 0x8000018b | UMBRU  | [Umbru](https://umbru.io)
396   | 0x8000018c | TON    | [Telegram Open Network](https://test.ton.org/)
397   | 0x8000018d | NEAR   | [NEAR Protocol](https://nearprotocol.com/)
398   | 0x8000018e | XPC    | [XPChain](https://www.xpchain.io/)
399   | 0x8000018f | ZOC    | [01coin](https://01coin.io/)
400   | 0x80000190 | NIX    | [NIX](https://nixplatform.io)
401   | 0x80000191 | UC     | [Utopiacoin](https://utopiacoin.org)
402   | 0x80000192 | GALI   | [Galilel](https://galilel.org/)
403   | 0x80000193 | OLT    | [Oneledger](https://www.oneledger.io/)
404   | 0x80000194 | XBI    | [XBI](https://bitcoinincognito.org)
405   | 0x80000195 | DONU   | [DONU](https://donu.io/)
406   | 0x80000196 | EARTHS | [Earths](https://earths.ga/)
407   | 0x80000197 | HDD    | [HDDCash](https://hdd.cash)
408   | 0x80000198 | SUGAR  | [Sugarchain](https://sugarchain.org/)
409   | 0x80000199 | AILE   | [AileCoin](https://ailecoin.com/)
410   | 0x8000019a | XSG    | [SnowGem](https://snowgem.org/)
411   | 0x8000019b | TAN    | [Tangerine Network](https://tangerine-network.io)
412   | 0x8000019c | AIN    | [AIN](https://www.ainetwork.ai)
413   | 0x8000019d | MSR    | [Masari](https://getmasari.org)
414   | 0x8000019e | SUMO   | [Sumokoin](https://www.sumokoin.org)
415   | 0x8000019f | ETN    | [Electroneum](https://electroneum.com)
416   | 0x800001a0 | SLX    | [SLX](https://slate.io/)
417   | 0x800001a1 | WOW    | [Wownero](http://wownero.org/)
418   | 0x800001a2 | XTNC   | [XtendCash](https://xtendcash.com/)
419   | 0x800001a3 | LTHN   | [Lethean](https://lethean.io/)
420   | 0x800001a4 | NODE   | [NodeHost](https://nodehost.online)
421   | 0x800001a5 | AGM    | [Argoneum](https://argoneum.net)
422   | 0x800001a6 | CCX    | [Conceal Network](https://conceal.network)
423   | 0x800001a7 | TNET   | [Title Network](https://title.network/)
424   | 0x800001a8 | TELOS  | [TelosCoin](https://teloscoin.org)
425   | 0x800001a9 | AION   | [Aion](https://aion.network)
426   | 0x800001aa | BC     | [Bitcoin Confidential](https://www.bitcoinconfidential.cc/)
427   | 0x800001ab | KTV    | [KmushiCoin](https://tierravivaplanet.com)
428   | 0x800001ac | ZCR    | [ZCore](https://zcore.cash)
429   | 0x800001ad | ERG    | [Ergo](https://ergoplatform.org)
430   | 0x800001ae | PESO   | [Criptopeso](https://criptopeso.io/)
431   | 0x800001af | BTC2   | [Bitcoin 2](https://www.bitc2.org/)
432   | 0x800001b0 | XRPHD  | [XRPHD](https://xrphd.org)
433   | 0x800001b1 | WE     | [WE Coin](https://we-corp.io)
434   | 0x800001b2 | KSM    | [Kusama](https://kusama.network)
435   | 0x800001b3 | PCN    | [Peepcoin](https://pxn.foundation/peepcoin)
436   | 0x800001b4 | NCH    | [NetCloth](https://www.netcloth.org)
437   | 0x800001b5 | ICU    | [CHIPO](http://www.chipo.icu)
438   | 0x800001b6 | LN     | [LINK](https://link.network/)
439   | 0x800001b7 | DTP    | [DeVault Token Protocol](https://devault.cc/token-protocol.html)
440   | 0x800001b8 | BTCR   | [Bitcoin Royale](https://bitcoinroyale.org)
441   | 0x800001b9 | AERGO  | [AERGO](https://www.aergo.io/)
442   | 0x800001ba | XTH    | [Dothereum](https://dothereum.net)
443   | 0x800001bb | LV     | [Lava](https://www.lavatech.org/)
444   | 0x800001bc | PHR    | [Phore](https://phore.io)
445   | 0x800001bd |        |
446   | 0x800001be |        |
447   | 0x800001bf | DIN    | [Dinero](https://dinerocoin.org/)
448   | 0x800001c0 |        |
449   | 0x800001c1 |        |
450   | 0x800001c2 | XLR    | [Solaris](https://solarisplatform.com)
451   | 0x800001c3 | KTS    | [Klimatas](https://www.klimatas.com)
452   | 0x800001c4 |        |
453   | 0x800001c5 |        |
454   | 0x800001c6 |        |
455   | 0x800001c7 |        |
456   | 0x800001c8 |        |
457   | 0x800001c9 | AE     | [æternity](https://aeternity.com)
458   | 0x800001ca |        |
459   | 0x800001cb |        |
460   | 0x800001cc |        |
461   | 0x800001cd |        |
462   | 0x800001ce |        |
463   | 0x800001cf |        |
464   | 0x800001d0 | ETI    | [EtherInc](https://einc.io)
465   | 0x800001d1 |        |
466   | 0x800001d2 |        |
467   | 0x800001d3 |        |
468   | 0x800001d4 |        |
469   | 0x800001d5 |        |
470   | 0x800001d6 |        |
471   | 0x800001d7 |        |
472   | 0x800001d8 |        |
473   | 0x800001d9 |        |
474   | 0x800001da |        |
475   | 0x800001db |        |
476   | 0x800001dc |        |
477   | 0x800001dd |        |
478   | 0x800001de |        |
479   | 0x800001df |        |
480   | 0x800001e0 |        |
481   | 0x800001e1 |        |
482   | 0x800001e2 |        |
483   | 0x800001e3 |        |
484   | 0x800001e4 |        |
485   | 0x800001e5 |        |
486   | 0x800001e6 |        |
487   | 0x800001e7 |        |
488   | 0x800001e8 | VEO    | [Amoveo](https://github.com/zack-bitcoin/amoveo/)
489   | 0x800001e9 |        |
490   | 0x800001ea |        |
491   | 0x800001eb |        |
492   | 0x800001ec |        |
493   | 0x800001ed |        |
494   | 0x800001ee |        |
495   | 0x800001ef |        |
496   | 0x800001f0 |        |
497   | 0x800001f1 |        |
498   | 0x800001f2 |        |
499   | 0x800001f3 |        |
500   | 0x800001f4 | THETA  | [Theta](https://www.thetatoken.org/)
501   | 0x800001f5 | SOL    | [Solana](https://solana.com)
502   | 0x800001f6 |        |
503   | 0x800001f7 |        |
504   | 0x800001f8 |        |
505   | 0x800001f9 |        |
506   | 0x800001fa |        |
507   | 0x800001fb |        |
508   | 0x800001fc |        |
509   | 0x800001fd |        |
510   | 0x800001fe | KOTO   | [Koto](https://ko-to.org/)
511   | 0x800001ff |        |
512   | 0x80000200 | XRD    | [Radiant](https://radiant.cash/)
513   | 0x80000201 |        |
514   | 0x80000202 |        |
515   | 0x80000203 |        |
516   | 0x80000204 | VEE    | [Virtual Economy Era](https://www.vee.tech/)
517   | 0x80000205 |        |
518   | 0x80000206 | LET    | [Linkeye](https://www.linkeye.com/)
519   | 0x80000207 |        |
520   | 0x80000208 | BTCV   | [BitcoinVIP](https://www.bitvip.org/)
521   | 0x80000209 |        |
522   | 0x8000020a |        |
523   | 0x8000020b |        |
524   | 0x8000020c |        |
525   | 0x8000020d |        |
526   | 0x8000020e | BU     | [BUMO](https://www.bumo.io/)
527   | 0x8000020f |        |
528   | 0x80000210 | YAP    | [Yapstone](https://yapstone.pro/)
529   | 0x80000211 |        |
530   | 0x80000212 |        |
531   | 0x80000213 |        |
532   | 0x80000214 |        |
533   | 0x80000215 | PRJ    | [ProjectCoin](https://projectcoin.net/)
534   | 0x80000216 |        |
535   | 0x80000217 |        |
536   | 0x80000218 |        |
537   | 0x80000219 |        |
538   | 0x8000021a |        |
539   | 0x8000021b |        |
540   | 0x8000021c |        |
541   | 0x8000021d |        |
542   | 0x8000021e |        |
543   | 0x8000021f |        |
544   | 0x80000220 |        |
545   | 0x80000221 |        |
546   | 0x80000222 |        |
547   | 0x80000223 |        |
548   | 0x80000224 |        |
549   | 0x80000225 |        |
550   | 0x80000226 |        |
551   | 0x80000227 |        |
552   | 0x80000228 |        |
553   | 0x80000229 |        |
554   | 0x8000022a |        |
555   | 0x8000022b | BCS    | [Bitcoin Smart](http://bcs.info)
556   | 0x8000022c |        |
557   | 0x8000022d | LKR    | [Lkrcoin](https://lkrcoin.io/)
558   | 0x8000022e |        |
559   | 0x8000022f |        |
560   | 0x80000230 |        |
561   | 0x80000231 | NTY    | [Nexty](https://nexty.io/)
562   | 0x80000232 |        |
563   | 0x80000233 |        |
564   | 0x80000234 |        |
565   | 0x80000235 |        |
566   | 0x80000236 |        |
567   | 0x80000237 |        |
568   | 0x80000238 |        |
569   | 0x80000239 |        |
570   | 0x8000023a |        |
571   | 0x8000023b |        |
572   | 0x8000023c |        |
573   | 0x8000023d |        |
574   | 0x8000023e |        |
575   | 0x8000023f |        |
576   | 0x80000240 |        |
577   | 0x80000241 |        |
578   | 0x80000242 |        |
579   | 0x80000243 |        |
580   | 0x80000244 |        |
581   | 0x80000245 |        |
582   | 0x80000246 |        |
583   | 0x80000247 |        |
584   | 0x80000248 |        |
585   | 0x80000249 |        |
586   | 0x8000024a |        |
587   | 0x8000024b |        |
588   | 0x8000024c |        |
589   | 0x8000024d |        |
590   | 0x8000024e |        |
591   | 0x8000024f |        |
592   | 0x80000250 |        |
593   | 0x80000251 |        |
594   | 0x80000252 |        |
595   | 0x80000253 |        |
596   | 0x80000254 |        |
597   | 0x80000255 |        |
598   | 0x80000256 |        |
599   | 0x80000257 |        |
600   | 0x80000258 | UTE    | [Unit-e](https://dtr.org/unit-e/)
601   | 0x80000259 |        |
602   | 0x8000025a |        |
603   | 0x8000025b |        |
604   | 0x8000025c |        |
605   | 0x8000025d |        |
606   | 0x8000025e |        |
607   | 0x8000025f |        |
608   | 0x80000260 |        |
609   | 0x80000261 |        |
610   | 0x80000262 |        |
611   | 0x80000263 |        |
612   | 0x80000264 |        |
613   | 0x80000265 |        |
614   | 0x80000266 |        |
615   | 0x80000267 |        |
616   | 0x80000268 |        |
617   | 0x80000269 |        |
618   | 0x8000026a | SSP    | [SmartShare](http://www.smartshare.vip/)
619   | 0x8000026b |        |
620   | 0x8000026c |        |
621   | 0x8000026d |        |
622   | 0x8000026e |        |
623   | 0x8000026f |        |
624   | 0x80000270 |        |
625   | 0x80000271 | EAST   | [Eastcoin](http://easthub.io/)
626   | 0x80000272 |        |
627   | 0x80000273 |        |
628   | 0x80000274 |        |
629   | 0x80000275 |        |
630   | 0x80000276 |        |
631   | 0x80000277 |        |
632   | 0x80000278 |        |
633   | 0x80000279 |        |
634   | 0x8000027a |        |
635   | 0x8000027b |        |
636   | 0x8000027c |        |
637   | 0x8000027d |        |
638   | 0x8000027e |        |
639   | 0x8000027f |        |
640   | 0x80000280 |        |
641   | 0x80000281 |        |
642   | 0x80000282 |        |
643   | 0x80000283 |        |
644   | 0x80000284 |        |
645   | 0x80000285 |        |
646   | 0x80000286 |        |
647   | 0x80000287 |        |
648   | 0x80000288 |        |
649   | 0x80000289 |        |
650   | 0x8000028a |        |
651   | 0x8000028b |        |
652   | 0x8000028c |        |
653   | 0x8000028d |        |
654   | 0x8000028e |        |
655   | 0x8000028f |        |
656   | 0x80000290 |        |
657   | 0x80000291 |        |
658   | 0x80000292 |        |
659   | 0x80000293 |        |
660   | 0x80000294 |        |
661   | 0x80000295 |        |
662   | 0x80000296 |        |
663   | 0x80000297 | SFRX   | [EtherGem Sapphire](https://egem.io)
664   | 0x80000298 |        |
665   | 0x80000299 |        |
666   | 0x8000029a | ACT    | [Achain](https://www.achain.com/)
667   | 0x8000029b | PRKL   | [Perkle](https://esprezzo.io/)
668   | 0x8000029c | SSC    | [SelfSell](https://www.selfsell.com/)
669   | 0x8000029d |        |
670   | 0x8000029e |        |
671   | 0x8000029f |        |
672   | 0x800002a0 |        |
673   | 0x800002a1 |        |
674   | 0x800002a2 |        |
675   | 0x800002a3 |        |
676   | 0x800002a4 |        |
677   | 0x800002a5 |        |
678   | 0x800002a6 |        |
679   | 0x800002a7 |        |
680   | 0x800002a8 |        |
681   | 0x800002a9 |        |
682   | 0x800002aa |        |
683   | 0x800002ab |        |
684   | 0x800002ac |        |
685   | 0x800002ad |        |
686   | 0x800002ae |        |
687   | 0x800002af |        |
688   | 0x800002b0 | CET    | [CoinEx Chain](https://www.coinex.org/)
689   | 0x800002b1 |        |
690   | 0x800002b2 |        |
691   | 0x800002b3 |        |
692   | 0x800002b4 |        |
693   | 0x800002b5 |        |
694   | 0x800002b6 |        |
695   | 0x800002b7 |        |
696   | 0x800002b8 |        |
697   | 0x800002b9 |        |
698   | 0x800002ba | VEIL   | [Veil](https://www.veil-project.com)
699   | 0x800002bb |        |
700   | 0x800002bc | XDAI   | [xDai](https://blockscout.com/poa/dai)
701   | 0x800002bd |        |
702   | 0x800002be |        |
703   | 0x800002bf |        |
704   | 0x800002c0 |        |
705   | 0x800002c1 |        |
706   | 0x800002c2 |        |
707   | 0x800002c3 |        |
708   | 0x800002c4 |        |
709   | 0x800002c5 |        |
710   | 0x800002c6 |        |
711   | 0x800002c7 |        |
712   | 0x800002c8 |        |
713   | 0x800002c9 | XTL    | [Katal Chain](https://katalchain.com)
714   | 0x800002ca | BNB    | [Binance](https://www.binance.org)
715   | 0x800002cb | SIN    | [Sinovate](https://sinovate.io)
716   | 0x800002cc |        |
717   | 0x800002cd |        |
718   | 0x800002ce |        |
719   | 0x800002cf |        |
720   | 0x800002d0 |        |
721   | 0x800002d1 |        |
722   | 0x800002d2 |        |
723   | 0x800002d3 |        |
724   | 0x800002d4 |        |
725   | 0x800002d5 |        |
726   | 0x800002d6 |        |
727   | 0x800002d7 |        |
728   | 0x800002d8 |        |
729   | 0x800002d9 |        |
730   | 0x800002da |        |
731   | 0x800002db |        |
732   | 0x800002dc |        |
733   | 0x800002dd |        |
734   | 0x800002de |        |
735   | 0x800002df |        |
736   | 0x800002e0 |        |
737   | 0x800002e1 |        |
738   | 0x800002e2 |        |
739   | 0x800002e3 |        |
740   | 0x800002e4 |        |
741   | 0x800002e5 |        |
742   | 0x800002e6 |        |
743   | 0x800002e7 |        |
744   | 0x800002e8 |        |
745   | 0x800002e9 |        |
746   | 0x800002ea |        |
747   | 0x800002eb |        |
748   | 0x800002ec |        |
749   | 0x800002ed |        |
750   | 0x800002ee |        |
751   | 0x800002ef |        |
752   | 0x800002f0 |        |
753   | 0x800002f1 |        |
754   | 0x800002f2 |        |
755   | 0x800002f3 |        |
756   | 0x800002f4 |        |
757   | 0x800002f5 |        |
758   | 0x800002f6 |        |
759   | 0x800002f7 |        |
760   | 0x800002f8 |        |
761   | 0x800002f9 |        |
762   | 0x800002fa |        |
763   | 0x800002fb |        |
764   | 0x800002fc |        |
765   | 0x800002fd |        |
766   | 0x800002fe |        |
767   | 0x800002ff |        |
768   | 0x80000300 | BALLZ  | [Ballzcoin](https://ballzcoin.org)
769   | 0x80000301 |        |
770   | 0x80000302 |        |
771   | 0x80000303 |        |
772   | 0x80000304 |        |
773   | 0x80000305 |        |
774   | 0x80000306 |        |
775   | 0x80000307 |        |
776   | 0x80000308 |        |
777   | 0x80000309 | BTW    | [Bitcoin World](http://btw.one)
778   | 0x8000030a |        |
779   | 0x8000030b |        |
780   | 0x8000030c |        |
781   | 0x8000030d |        |
782   | 0x8000030e |        |
783   | 0x8000030f |        |
784   | 0x80000310 |        |
785   | 0x80000311 |        |
786   | 0x80000312 |        |
787   | 0x80000313 |        |
788   | 0x80000314 |        |
789   | 0x80000315 |        |
790   | 0x80000316 |        |
791   | 0x80000317 |        |
792   | 0x80000318 |        |
793   | 0x80000319 |        |
794   | 0x8000031a |        |
795   | 0x8000031b |        |
796   | 0x8000031c |        |
797   | 0x8000031d |        |
798   | 0x8000031e |        |
799   | 0x8000031f |        |
800   | 0x80000320 | BEET   | [Beetle Coin](https://beetlecoin.io/)
801   | 0x80000321 | DST    | [DSTRA](https://dstra.io/)
802   | 0x80000322 |        |
803   | 0x80000323 |        |
804   | 0x80000324 |        |
805   | 0x80000325 |        |
806   | 0x80000326 |        |
807   | 0x80000327 |        |
808   | 0x80000328 | QVT    | [Qvolta](https://qvolta.com)
809   | 0x80000329 |        |
810   | 0x8000032a |        |
811   | 0x8000032b |        |
812   | 0x8000032c |        |
813   | 0x8000032d |        |
814   | 0x8000032e |        |
815   | 0x8000032f |        |
816   | 0x80000330 |        |
817   | 0x80000331 |        |
818   | 0x80000332 | VET    | [VeChain Token](https://vechain.com/)
819   | 0x80000333 |        |
820   | 0x80000334 | CLO    | [Callisto](http://callisto.network/)
821   | 0x80000335 |        |
822   | 0x80000336 |        |
823   | 0x80000337 |        |
824   | 0x80000338 |        |
825   | 0x80000339 |        |
826   | 0x8000033a |        |
827   | 0x8000033b |        |
828   | 0x8000033c |        |
829   | 0x8000033d |        |
830   | 0x8000033e |        |
831   | 0x8000033f | CRUZ   | [cruzbit](https://github.com/cruzbit/cruzbit)
832   | 0x80000340 |        |
833   | 0x80000341 |        |
834   | 0x80000342 |        |
835   | 0x80000343 |        |
836   | 0x80000344 |        |
837   | 0x80000345 |        |
838   | 0x80000346 |        |
839   | 0x80000347 |        |
840   | 0x80000348 |        |
841   | 0x80000349 |        |
842   | 0x8000034a |        |
843   | 0x8000034b |        |
844   | 0x8000034c |        |
845   | 0x8000034d |        |
846   | 0x8000034e |        |
847   | 0x8000034f |        |
848   | 0x80000350 |        |
849   | 0x80000351 |        |
850   | 0x80000352 |        |
851   | 0x80000353 |        |
852   | 0x80000354 | DESM   | [Desmos](https://github.com/desmos-labs/introduction)
853   | 0x80000355 |        |
854   | 0x80000356 |        |
855   | 0x80000357 |        |
856   | 0x80000358 |        |
857   | 0x80000359 |        |
858   | 0x8000035a |        |
859   | 0x8000035b |        |
860   | 0x8000035c |        |
861   | 0x8000035d |        |
862   | 0x8000035e |        |
863   | 0x8000035f |        |
864   | 0x80000360 |        |
865   | 0x80000361 |        |
866   | 0x80000362 |        |
867   | 0x80000363 |        |
868   | 0x80000364 |        |
869   | 0x80000365 |        |
870   | 0x80000366 |        |
871   | 0x80000367 |        |
872   | 0x80000368 |        |
873   | 0x80000369 |        |
874   | 0x8000036a |        |
875   | 0x8000036b |        |
876   | 0x8000036c |        |
877   | 0x8000036d |        |
878   | 0x8000036e |        |
879   | 0x8000036f |        |
880   | 0x80000370 |        |
881   | 0x80000371 |        |
882   | 0x80000372 |        |
883   | 0x80000373 |        |
884   | 0x80000374 |        |
885   | 0x80000375 |        |
886   | 0x80000376 | ADF    | [AD Token](http://adfunds.org)
887   | 0x80000377 |        |
888   | 0x80000378 | NEO    | [NEO](https://neo.org/)
889   | 0x80000379 | TOMO   | [TOMO](https://tomochain.com/)
890   | 0x8000037a | XSEL   | [Seln](https://selnx.jp/)
891   | 0x8000037b |        |
892   | 0x8000037c |        |
893   | 0x8000037d |        |
894   | 0x8000037e |        |
895   | 0x8000037f |        |
896   | 0x80000380 |        |
897   | 0x80000381 |        |
898   | 0x80000382 |        |
899   | 0x80000383 |        |
900   | 0x80000384 | LMO    | [Lumeneo](https://lumeneo.network/)
901   | 0x80000385 |        |
902   | 0x80000386 |        |
903   | 0x80000387 |        |
904   | 0x80000388 |        |
905   | 0x80000389 |        |
906   | 0x8000038a |        |
907   | 0x8000038b |        |
908   | 0x8000038c |        |
909   | 0x8000038d |        |
910   | 0x8000038e |        |
911   | 0x8000038f |        |
912   | 0x80000390 |        |
913   | 0x80000391 |        |
914   | 0x80000392 |        |
915   | 0x80000393 |        |
916   | 0x80000394 | META   | [Metadium](https://www.metadium.com/)
917   | 0x80000395 |        |
918   | 0x80000396 |        |
919   | 0x80000397 |        |
920   | 0x80000398 |        |
921   | 0x80000399 |        |
922   | 0x8000039a |        |
923   | 0x8000039b |        |
924   | 0x8000039c |        |
925   | 0x8000039d |        |
926   | 0x8000039e |        |
927   | 0x8000039f |        |
928   | 0x800003a0 |        |
929   | 0x800003a1 |        |
930   | 0x800003a2 |        |
931   | 0x800003a3 |        |
932   | 0x800003a4 |        |
933   | 0x800003a5 |        |
934   | 0x800003a6 |        |
935   | 0x800003a7 |        |
936   | 0x800003a8 |        |
937   | 0x800003a9 |        |
938   | 0x800003aa |        |
939   | 0x800003ab |        |
940   | 0x800003ac |        |
941   | 0x800003ad |        |
942   | 0x800003ae |        |
943   | 0x800003af |        |
944   | 0x800003b0 |        |
945   | 0x800003b1 |        |
946   | 0x800003b2 |        |
947   | 0x800003b3 |        |
948   | 0x800003b4 |        |
949   | 0x800003b5 |        |
950   | 0x800003b6 |        |
951   | 0x800003b7 |        |
952   | 0x800003b8 |        |
953   | 0x800003b9 |        |
954   | 0x800003ba |        |
955   | 0x800003bb |        |
956   | 0x800003bc |        |
957   | 0x800003bd |        |
958   | 0x800003be |        |
959   | 0x800003bf |        |
960   | 0x800003c0 |        |
961   | 0x800003c1 |        |
962   | 0x800003c2 |        |
963   | 0x800003c3 |        |
964   | 0x800003c4 |        |
965   | 0x800003c5 |        |
966   | 0x800003c6 |        |
967   | 0x800003c7 |        |
968   | 0x800003c8 |        |
969   | 0x800003c9 |        |
970   | 0x800003ca | TWINS  | [TWINS](https://win.win/)
971   | 0x800003cb |        |
972   | 0x800003cc |        |
973   | 0x800003cd |        |
974   | 0x800003ce |        |
975   | 0x800003cf |        |
976   | 0x800003d0 |        |
977   | 0x800003d1 |        |
978   | 0x800003d2 |        |
979   | 0x800003d3 |        |
980   | 0x800003d4 |        |
981   | 0x800003d5 |        |
982   | 0x800003d6 |        |
983   | 0x800003d7 |        |
984   | 0x800003d8 |        |
985   | 0x800003d9 |        |
986   | 0x800003da |        |
987   | 0x800003db |        |
988   | 0x800003dc |        |
989   | 0x800003dd |        |
990   | 0x800003de |        |
991   | 0x800003df |        |
992   | 0x800003e0 |        |
993   | 0x800003e1 |        |
994   | 0x800003e2 |        |
995   | 0x800003e3 |        |
996   | 0x800003e4 | OKP    | [OK Points](https://www.okcoin.com/chain)
997   | 0x800003e5 | SUM    | [Solidum](https://solidum.network)
998   | 0x800003e6 | LBTC   | [Lightning Bitcoin](http://lbtc.io/)
999   | 0x800003e7 | BCD    | [Bitcoin Diamond](http://btcd.io/)
1000  | 0x800003e8 | BTN    | [Bitcoin New](http://bitcoinnew.org/)
1001  | 0x800003e9 | TT     | [ThunderCore](https://thundercore.com/)
1002  | 0x800003ea | BKT    | [BanKitt](https://www.bankitt.network/)
1023  | 0x800003ff | ONE    | [HARMONY-ONE](https://harmony.one/)
1024  | 0x80000400 | ONT    | [Ontology](https://ont.io)
1026  | 0x80000402 | KEX    | [Kira Exchange Token](https://kiraex.com)
1027  | 0x80000403 | MCM    | [Mochimo](https://mochimo.org)
1111  | 0x80000457 | BBC    | [Big Bitcoin](http://bigbitcoins.org/)
1120  | 0x80000460 | RISE   | [RISE](https://rise.vision)
1122  | 0x80000462 | CMT    | [CyberMiles Token](https://www.cybermiles.io)
1128  | 0x80000468 | ETSC   | [Ethereum Social](https://ethereumsocial.kr/)
1145  | 0x80000479 | CDY    | [Bitcoin Candy](http://www.bitcoincandy.one)
1337  | 0x80000539 | DFC    | [Defcoin](http://defcoin-ng.org)
1397  | 0x80000575 | HYC    | [Hycon](https://hycon.io)
1524  | 0x800005f4 |        | [Taler](http://taler.site)
1533  | 0x800005fd | BEAM   | [Beam](https://www.beam.mw/)
1616  | 0x80000650 | ELF    | [AELF](https://aelf.io)
1620  | 0x80000654 | ATH    | [Atheios](https://atheios.com)
1688  | 0x80000698 | BCX    | [BitcoinX](https://bcx.org)
1729  | 0x800006c1 | XTZ    | [Tezos](https://tezos.com)
1776  | 0x800006f0 | LBTC   | [Liquid BTC](https://blockstream.com/liquid/)
1815  | 0x80000717 | ADA    | [Cardano](https://www.cardanohub.org/en/home/)
1856  | 0x80000743 | TES    | [Teslacoin](https://www.tesla-coin.com/)
1901  | 0x8000076d | CLC    | [Classica](https://github.com/classica/)
1919  | 0x8000077f | VIPS   | [VIPSTARCOIN](https://www.vipstarcoin.jp/)
1926  | 0x80000786 | CITY   | [City Coin](https://city-chain.org/)
1977  | 0x800007b9 | XMX    | [Xuma](http://www.xumacoin.org/)
1984  | 0x800007c0 | TRTL   | [TurtleCoin](https://turtlecoin.lol/)
1987  | 0x800007c3 | EGEM   | [EtherGem](https://egem.io)
1989  | 0x800007c5 | HODL   | [HOdlcoin](https://hodlcoin.com/)
1990  | 0x800007c6 | PHL    | [Placeholders](https://placeh.io/)
1997  | 0x800007cd | POLIS  | [Polis](https://polispay.org/)
1998  | 0x800007ce | XMCC   | [Monoeci](https://monoeci.io/)
1999  | 0x800007cf | COLX   | [ColossusXT](https://colossusxt.io/)
2000  | 0x800007d0 | GIN    | [GinCoin](https://gincoin.io/)
2001  | 0x800007d1 | MNP    | [MNPCoin](https://mnpcoin.pro/)
2017  | 0x800007e1 | KIN    | [Kin](https://www.kinecosystem.org/)
2018  | 0x800007e2 | EOSC   | [EOSClassic](https://eos-classic.io/)
2019  | 0x800007e3 | GBT    | [GoldBean Token](http://www.adfunds.org/)
2020  | 0x800007e4 | PKC    | [PKC](https://www.pkc.ink/)
2048  | 0x80000800 | MCASH  | [MCashChain](https://mcash.network/)
2049  | 0x80000801 | TRUE   | [TrueChain](https://www.truechain.pro/)
2112  | 0x80000840 | IoTE   | [IoTE](https://www.iote.one/)
2221  | 0x800008ad | ASK    | [ASK](https://permission.io/)
2301  | 0x800008fd | QTUM   | [QTUM](https://qtum.org/en/)
2302  | 0x800008fe | ETP    | [Metaverse](https://mvs.org/)
2303  | 0x800008ff | GXC    | [GXChain](https://www.gxb.io)
2304  | 0x80000900 | CRP    | [CranePay](https://cranepay.io)
2305  | 0x80000901 | ELA    | [Elastos](https://www.elastos.org/)
2338  | 0x80000922 | SNOW   | [Snowblossom](https://snowblossom.org/)
2570  | 0x80000a0a | AOA    | [Aurora](https://www.aurorachain.io/)
2894  | 0x80000b4e | REOSC  | [REOSC Ecosystem](https://www.reosc.io/)
3003  | 0x80000bbb | LUX    | [LUX](https://luxcore.io/)
3030  | 0x80000bd6 | XHB    | [Hedera HBAR](https://www.hedera.com/)
3077  | 0x80000c05 | COS    | [Contentos](https://www.contentos.io/)
3381  | 0x80000d35 | DYN    | [Dynamic](https://duality.solutions/dynamic/)
3383  | 0x80000d37 | SEQ    | [Sequence](https://duality.solutions/sequence/)
3552  | 0x80000de0 | DEO    | [Destocoin](https://desto.io)
3564  | 0x80000dec | DST    | [DeStream](https://destream.io)
2718  | 0x80000a9e | NAS    | [Nebulas](https://nebulas.io/)
2941  | 0x80000b7d | BND    | [Blocknode](https://blocknode.tech)
3276  | 0x80000ccc | CCC    | [CodeChain](https://codechain.io/)
3377  | 0x80000d31 | ROI    | [ROIcoin](https://roi-coin.com/)
4096  | 0x80001000 | YEE    | [YeeCo](https://www.yeeco.io/)
4218  | 0x8000107a | IOTA   | [IOTA](https://www.iota.org/)
4242  | 0x80001092 | AXE    | [Axe](https://github.com/AXErunners/axe)
5248  | 0x00001480 | FIC    | [FIC](https://ficnetwork.com)
5353  | 0x000014e9 | HNS    | [Handshake](https://handshake.org)
5757  | 0x8000167d |        | [Stacks](https://github.com/blockstack/blockstack-core)
5920  | 0x80001720 | SLU    | [SILUBIUM](https://github.com/SilubiumProject/slucore)
6060  | 0x800017ac | GO     | [GoChain GO](https://gochain.io/)
6666  | 0x80001a0a | BPA    | [Bitcoin Pizza](http://p.top/)
6688  | 0x80001a20 | SAFE   | [SAFE](http://www.anwang.com/)
6969  | 0x80001b39 | ROGER  | [TheHolyrogerCoin](https://github.com/TheHolyRoger/TheHolyRogerCoin)
7777  | 0x80001e61 | BTV    | [Bitvote](https://www.bitvote.one)
8000  | 0x80001f40 | SKY    | [Skycoin](https://www.skycoin.net)
8339  | 0x80002093 | BTQ    | [BitcoinQuark](https://www.bitcoinquark.org)
8888  | 0x800022b8 | SBTC   | [Super Bitcoin](https://www.superbtc.org)
8964  | 0x80002304 | NULS   | [NULS](https://nuls.io)
8999  | 0x80002327 | BTP    | [Bitcoin Pay](http://www.btceasypay.com)
9797  | 0x80002645 | NRG    | [Energi](https://www.energi.world/)
9888  | 0x800026a0 | BTF    | [Bitcoin Faith](http://bitcoinfaith.org)
9999  | 0x8000270f | GOD    | [Bitcoin God](https://www.bitcoingod.org)
10000 | 0x80002710 | FO     | [FIBOS](https://fibos.io/)
10291 | 0x80002833 | BTR    | [Bitcoin Rhodium](https://www.bitcoinrh.org)
11111 | 0x80002b67 | ESS    | [Essentia One](https://essentia.one/)
12345 | 0x80003039 | IPOS   | [IPOS](https://iposlab.com)
13107 | 0x80003333 | BTY    | [BitYuan](https://www.bityuan.com)
13108 | 0x80003334 | YCC    | [Yuan Chain Coin](https://www.yuan.org)
15845 | 0x80003de5 | SDGO   | [SanDeGo](http://www.sandego.net)
16754 | 0x80004172 | ARDR   | [Ardor](https://www.jelurida.com)
19165 | 0x80004add | SAFE   | [Safecoin](https://www.safecoin.org)
19167 | 0x80004adf | ZEL    | [ZelCash](https://www.zel.cash)
19169 | 0x80004ae1 | RITO   | [Ritocoin](https://www.ritocoin.org)
20036 | 0x80004e44 | XND    | [ndau](https://ndau.io/)
22504 | 0x800057e8 | PWR    | [PWRcoin](https://github.com/Plainkoin/PWRcoin)
25252 | 0x800062a4 | BELL   | [Bellcoin](https://bellcoin.web4u.jp/)
25718 | 0x80006476 | CHX    | [Own](https://wallet.weown.com)
31102 | 0x8000797e | ESN    | [EtherSocial Network](https://ethersocial.network)
31337 | 0x80007a69 |        | [ThePower.io](https://thepower.io)
33416 | 0x80008288 | TEO    | [Trust Eth reOrigin](https://tao.foundation)
33878 | 0x80008456 | BTCS   | [Bitcoin Stake](http://www.btcscoin.com/)
34952 | 0x80008888 | BTT    | [ByteTrade](https://bytetrade.io/)
37992 | 0x80009468 | FXTC   | [FixedTradeCoin](https://fixedtradecoin.org/)
39321 | 0x80009999 | AMA    | [Amabig](https://amabig.com/)
49262 | 0x8000c06e | EVE    | [evan.network](https://evan.network/)
49344 | 0x0000c0c0 | STASH  | [STASH](https://stashpay.io/)
65536 | 0x80010000 | KETH   | [Krypton World](http:/krypton.world/)
88888 | 0x80015b38 | RYO    | [c0ban](https://www.c0ban.co/)
99999 | 0x8001869f | WICC   | [Waykichain](http://www.waykichain.com)
200625 | 0x80030fb1 | AKA    | [Akroma](https://akroma.io)
200665 | 0x80011000 | GENOM  | [GENOM](https://genom.tech)
246529 | 0x8003c301 | ATS    | [ARTIS sigma1](https://artis.eco/)
424242 | 0x80067932 | X42    | [x42](http://www.x42.tech)
666666 | 0x800a2c2a | VITE   | [Vite](https://www.vite.org)
1171337 | 0x8011df89 | ILT    | [iOlite](https://iolite.io/)
1313114 | 0x8014095a | ETHO   | [Ether-1](https://www.ether1.org)
1313500 | 0x80140adc | XERO   | [Xerom](https://www.xerom.org)
1712144 | 0x801a2010 | LAX    | [LAPO](https://lapo.io)
5249353 | 0x80501949 | BCO    | [BitcoinOre](http://bitcoinore.org/)
5249354 | 0x8050194a | BHD    | [BitcoinHD](https://btchd.org)
5264462 | 0x8050544e | PTN    | [PalletOne](https://pallet.one/)
5718350 | 0x8057414e | WAN    | [Wanchain](https://wanchain.org/)
5741564 | 0x80579bfc | WAVES  | [Waves](https://wavesplatform.com/)
7562605 | 0x8073656d | SEM    | [Semux](https://semux.org)
7567736 | 0x80737978 | ION    | [ION](https://ionomy.com/)
7825266 | 0x80776772 | WGR    | [WGR](https://wagerr.com)
7825267 | 0x80776773 | OBSR   | [OBServer](https://obsr.org/)
61717561 | 0x83adbc39 | AQUA   | [Aquachain](https://aquachain.github.io/)
88888888 | 0x854c5638 | HATCH  | [Hatch](https://hatch.ga/)
91927009 | 0x857ab1e1 | kUSD   | [kUSD](https://kowala.tech)
99999998 | 0x85f5e0fe | FLUID  | [Fluid Chains](https://www.fluidchains.com)
99999999 | 0x85f5e0ff | QKC    | [QuarkChain](https://www.quarkchain.io)";
    }

    
}


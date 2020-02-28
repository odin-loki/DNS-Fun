using System;
using System.Collections.Generic;
using System.Text;

namespace DWorks
{
    public class GScrape
    {
        /*
            source=
            ei=
            gs_l=

            https://web.archive.org/web/20121125075932/http://www.mkbergman.com/1009/deconstructing-the-google-knowledge-graph/
             https://gist.github.com/sshay77/4b1f6616a7afabc1ce2a
             https://web.archive.org/web/20121114093453/http://www.rankpanel.com/blog/google-search-parameters
        */
        private class SearchParameters
        {
            string homeURL = "https://www.google.com/webhp";
            string baseURL = "https://www.google.com/search?";

            char conact = '+';
            char seperator = '&';

            string queryString = "q=";
            string resultsInclude = "as_epq=";
            string ORresultsInclude = "as_oq=";
            string dontInclude = "as_eq=";
            string numResukts = "num=";
            string resultFiletype = "as_filetype=";
            string asWebsiteOnly = "as_sitesearch=";
            string indexedInLast = "as_qdr="; //d, w, m, y, mn eg. m2 = 2 months.
            string legalRights = "as_rights="; //See Legal below.
            string allInTitle = "allintitle%"; //This is actually appended to the q= parameter,
            string numericRange = "nnn..yyyy"; //See Numeric
            string plusTerm = "%2B"; //See Term, +term, ~term
            char tildeTerm = '~'; //
            string defineWord = "define%3A";
            //Calculate Functions
            string safeSearch = "safe="; //active or images
            string relatedResults = "as_rq=";
            string linkedResults = "as_lq=";
            string openNewWindow = "newwindow="; //1 active, 0 off
            string personalisedSearch = "pws"; //1 active, 0 off
            string adWordsOn = "adtest="; //on or off
            string buttonClick = "btnG="; //Search or btnI 
            string inputEncoding = "ie="; //default utf-8, set server-side
            string outputEncdoing = "oe="; //default utf-8, set server-side
            string languageValue = "lr="; //Enum
            string countryValue = "cr=country"; //enum

            //Case to string with nameof(enum.num)

            enum SafeSeach
            {
                active, //get enum value
                images
            }

            enum NewWindow
            {
                off, //get enum value
                on
            }

            enum PersonalisedSearch
            {
                off,
                on
            }

            enum AdTest
            {
                off, //get by name
                on
            }



            enum ButtonClick
            {
                Search,
                btnl
            }

            enum LanguageValue
            {
                lang_ar,
                lang_hy,
                lang_be,
                lang_bg,
                lang_ca,
                lang_hr,
                lang_cs,
                lang_da,
                lang_nl,
                lang_en,
                lang_eo,
                lang_et,
                lang_tl,
                lang_fi,
                lang_fr,
                lang_de,
                lang_el,
                lang_iw,
                lang_hu,
                lang_is,
                lang_id,
                lang_it,
                lang_ja,
                lang_ko,
                lang_lv,
                lang_lt,
                lang_no,
                lang_fa,
                lang_pl,
                lang_pt,
                lang_ro,
                lang_ru,
                lang_sr,
                lang_sk,
                lang_sl,
                lang_es,
                lang_sv,
                lang_th,
                lang_tr,
                lang_uk,
                lang_vi,
                lang_zh
            }

            enum Chinese
            {
                CN,
                TW
            }

            enum CountryValue
            {
                AD,
                AE,
                AF,
                AG,
                AI,
                AL,
                AM,
                AN,
                AO,
                AQ,
                AR,
                AS,
                AT,
                AU,
                AW,
                AZ,
                BA,
                BB,
                BD,
                BE,
                BF,
                BG,
                BH,
                BI,
                BJ,
                BM,
                BN,
                BO,
                BR,
                BS,
                BT,
                BV,
                BW,
                BY,
                BZ,
                CA,
                CC,
                CD,
                CF,
                CG,
                CH,
                CI,
                CK,
                CL,
                CM,
                CN,
                CO,
                CR,
                CS,
                CV,
                CX,
                CY,
                CZ,
                DE,
                DJ,
                DK,
                DM,
                DO,
                DZ,
                EC,
                EE,
                EG,
                EH,
                ER,
                ES,
                ET,
                FI,
                FJ,
                FK,
                FM,
                FO,
                FR,
                GA,
                GB,
                GD,
                GE,
                GF,
                GH,
                GI,
                GL,
                GM,
                GN,
                GP,
                GQ,
                GR,
                GS,
                GT,
                GU,
                GW,
                GY,
                HK,
                HM,
                HN,
                HR,
                HT,
                HU,
                ID,
                IE,
                IL,
                IN,
                IO,
                IQ,
                IS,
                IT,
                JM,
                JO,
                JP,
                KE,
                KG,
                KH,
                KI,
                KM,
                KN,
                KR,
                KW,
                KY,
                KZ,
                LA,
                LB,
                LC,
                LI,
                LK,
                LR,
                LS,
                LT,
                LU,
                LV,
                LY,
                MA,
                MC,
                MD,
                MG,
                MH,
                MK,
                ML,
                MN,
                MO,
                MP,
                MQ,
                MR,
                MS,
                MT,
                MU,
                MV,
                MW,
                MX,
                MY,
                MZ,
                NA,
                NC,
                NE,
                NF,
                NG,
                NI,
                NL,
                NO,
                NP,
                NR,
                NU,
                NZ,
                OM,
                PA,
                PE,
                PF,
                PG,
                PH,
                PK,
                PL,
                PM,
                PN,
                PR,
                PS,
                PT,
                PW,
                PY,
                QA,
                RE,
                RO,
                RU,
                RW,
                SA,
                SB,
                SC,
                SE,
                SG,
                SH,
                SI,
                SJ,
                SK,
                SL,
                SM,
                SN,
                SO,
                SR,
                ST,
                SV,
                SZ,
                TC,
                TD,
                TF,
                TG,
                TH,
                TJ,
                TK,
                TL,
                TM,
                TN,
                TO,
                TR,
                TT,
                TV,
                TW,
                TZ,
                UA,
                UG,
                UM,
                US,
                UY,
                UZ,
                VA,
                VC,
                VE,
                VG,
                VI,
                VN,
                VU,
                WF,
                WS,
                YE,
                YT,
                ZA,
                ZM,
                ZW
            }
        }


        //Build Google Querys and Post.

        //Spoof User Agents. (Fuck your Proxy). Use one for enitre session.

        //Make a base google request every 30 seconds. (Simulate normal browsing)
        //Eg. Open Browser to homepage -> Submit Query -> Download links -> Next Page -> Next 100 pages -> Try a new Query -> Open Browser to Homepage -> Repeat Again.
        //All google nuts open to google and search at lightspeed, the key is replicate what you normally do in a program. Only query one Subject matter.

        //Rate limit to 10 requests in a minute at random.

        //Download entire strings.

        //Extract Html links by regex.


        /*
         LEGAL:

        (cc_publicdomain|cc_attribute|cc_sharealike|cc_noncommercial|cc_nonderived) - free to use or share
        (cc_publicdomain|cc_attribute|cc_sharealike|cc_nonderived).-(cc_noncommercial) - free to use or share, including commercially
        (cc_publicdomain|cc_attribute|cc_sharealike|cc_noncommercial).-(cc_nonderived) - free to use, share, or modify
        (cc_publicdomain|cc_attribute|cc_sharealike).-(cc_noncommercial|cc_nonderived) - free to use, share, or modify commercially
         
        NUMERIC:
        Like the allin parameters, this is actually appended to the q= parameter. What this does though is let you search for results between numeric ranges. For example, if you wanted to find documents with numbers between 15 and 100, you'd put in 15..100. Very useful for finding products in a price range, when combined with the site limiter. Works with $, £, and other such things.
         
        TERM:
        %2Bterm
        Again, this is appended to the q= parameter. The %2B is actually the + sign encoded, and will return results featuring only the term used, with no pluralisations, alternate tenses, or synonyms.
        Shows as +term
        ~term
        Another one that's appended to the q= parameter. Returns results for the term used and synonyms.
        Shows as ~term

        CACLUCATE:

        term * term two
        And another q= parameter add-on. Returns results with listings that contain both words, with other words between them. 
        n+n2, n-n2, n/n2, n*n2, n^n2 and n% of n2
        Google's calculator functions. They are, in order, add, subtract, divide, multiply, raise to the power of, and return x percentage of.


         */


    }
}

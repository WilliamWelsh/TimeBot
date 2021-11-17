using System.Collections.Generic;

namespace TimeBot
{
    public static class Countries
    {
        public static List<string> List = new()
        {
            "Afghanistan",
            "Albania",
            "Algeria",
            "Andorra",
            "Angola",
            "Antigua and Barbuda",
            "Argentina",
            "Armenia",
            "Australia",
            "Austria",
            "Azerbaijan",
            "The Bahamas",
            "Bahrain",
            "Bangladesh",
            "Barbados",
            "Belarus",
            "Belgium",
            "Belize",
            "Benin",
            "Bhutan",
            "Bolivia",
            "Bosnia and Herzegovina",
            "Botswana",
            "Brazil",
            "Brunei",
            "Bulgaria",
            "Burkina Faso",
            "Burundi",
            "Cabo Verde",
            "Cambodia",
            "Cameroon",
            "Canada",
            "Central African Republic",
            "Chad",
            "Chile",
            "China",
            "Colombia",
            "Comoros",
            "Democratic Republic of the Congo",
            "Republic of the Congo",
            "Costa Rica",
            "Côte d’Ivoire",
            "Croatia",
            "Cuba",
            "Cyprus",
            "Czech Republic",
            "Denmark",
            "Djibouti",
            "Dominica",
            "Dominican Republic",
            "East Timor (Timor-Leste)",
            "Ecuador",
            "Egypt",
            "El Salvador",
            "Equatorial Guinea",
            "Eritrea",
            "Estonia",
            "Ethiopia",
            "Fiji",
            "Finland",
            "France",
            "Gabon",
            "The Gambia",
            "Georgia",
            "Germany",
            "Ghana",
            "Greece",
            "Grenada",
            "Guatemala",
            "Guinea",
            "Guinea-Bissau",
            "Guyana",
            "Haiti",
            "Honduras",
            "Hong Kong",
            "Hungary",
            "Iceland",
            "India",
            "Indonesia",
            "Iran",
            "Iraq",
            "Ireland",
            "Israel",
            "Italy",
            "Jamaica",
            "Japan",
            "Jordan",
            "Kazakhstan",
            "Kenya",
            "Kiribati",
            "North Korea",
            "South Korea",
            "Kosovo",
            "Kuwait",
            "Kyrgyzstan",
            "Laos",
            "Latvia",
            "Lebanon",
            "Lesotho",
            "Liberia",
            "Libya",
            "Liechtenstein",
            "Lithuania",
            "Luxembourg",
            "Macedonia",
            "Madagascar",
            "Malawi",
            "Malaysia",
            "Maldives",
            "Mali",
            "Malta",
            "Marshall Islands",
            "Mauritania",
            "Mauritius",
            "Mexico",
            "Federated States of Micronesia",
            "Moldova",
            "Monaco",
            "Mongolia",
            "Montenegro",
            "Morocco",
            "Mozambique",
            "Myanmar",
            "Namibia",
            "Nauru",
            "Nepal",
            "Netherlands",
            "New Zealand",
            "Nicaragua",
            "Niger",
            "Nigeria",
            "Norway",
            "Oman",
            "Pakistan",
            "Palau",
            "Panama",
            "Papua New Guinea",
            "Paraguay",
            "Peru",
            "Philippines",
            "Poland",
            "Portugal",
            "Qatar",
            "Romania",
            "Russia",
            "Rwanda",
            "Saint Kitts and Nevis",
            "Saint Lucia",
            "Saint Vincent and the Grenadines",
            "Samoa",
            "San Marino",
            "Sao Tome and Principe",
            "Saudi Arabia",
            "Scotland",
            "Senegal",
            "Serbia",
            "Seychelles",
            "Sierra Leone",
            "Singapore",
            "Slovakia",
            "Slovenia",
            "Solomon Islands",
            "Somalia",
            "South Africa",
            "Spain",
            "Sri Lanka",
            "Sudan",
            "Sudan, South",
            "Suriname",
            "Swaziland",
            "Sweden",
            "Switzerland",
            "Syria",
            "Taiwan",
            "Tajikistan",
            "Tanzania",
            "Thailand",
            "Togo",
            "Tonga",
            "Trinidad and Tobago",
            "Tunisia",
            "Turkey",
            "Turkmenistan",
            "Tuvalu",
            "Uganda",
            "Ukraine",
            "United Arab Emirates",
            "United Kingdom",
            "United States",
            "Uruguay",
            "Uzbekistan",
            "Vanuatu",
            "Vatican City",
            "Venezuela",
            "Vietnam",
            "Yemen",
            "Zambia",
            "Zimbabwe",
        };

        /// <summary>
        /// Get the emoji for a country
        /// </summary>
        public static string GetFlagEmoji(string country)
        {
            switch (country)
            {
                case "Australia":
                    return "🇦🇺";

                case "Canada":
                    return "🇨🇦";

                case "China":
                    return "🇨🇳";

                case "Djibouti":
                    return "🇩🇯";

                case "Latvia":
                    return "🇱🇻";

                case "Germany":
                    return "🇩🇪";

                case "France":
                    return "🇫🇷";

                case "Poland":
                    return "🇵🇱";

                case "Mexico":
                    return "🇲🇽";

                case "Turkey":
                    return "🇹🇷";

                case "Uruguay":
                    return "🇺🇾";

                case "Philippines":
                    return "🇵🇭";

                case "Denmark":
                    return "🇩🇰";

                case "Netherlands":
                    return "🇳🇱";

                case "Scotland":
                    return "🏴󠁧󠁢󠁳󠁣󠁴󠁿";

                case "Sweden":
                    return "🇸🇪";

                case "United Kingdom":
                    return "🇬🇧";

                case "United States":
                    return "🇺🇸";

                case "Bangladesh":
                    return "🇧🇩";

                case "Ethiopia":
                    return "🇪🇹";

                case "India":
                    return "🇮🇳";

                case "Indonesia":
                    return "🇮🇩";

                case "Lebanon":
                    return "🇱🇧";

                case "Morocco":
                    return "🇲🇦";

                case "Norway":
                    return "🇳🇴";

                case "Pakistan":
                    return "🇵🇰";

                case "Ukraine":
                    return "🇺🇦";

                case "Singapore":
                    return "🇸🇬";

                case "Ireland":
                    return "🇮🇪";

                case "Dominica":
                    return "🇩🇲";

                case "Malaysia":
                    return "🇲🇾";

                case "Nepal":
                    return "🇳🇵";

                case "Hong Kong":
                    return "🇭🇰";

                case "Vietnam":
                    return "🇻🇳";

                case "Nigeria":
                    return "🇳🇬";

                case "Belgium":
                    return "🇧🇪";

                case "Ghana":
                    return "🇬🇭";

                case "Jamaica":
                    return "🇯🇲";

                case "Jordan":
                    return "🇯🇴";

                case "Barbados":
                    return "🇧🇧";

                case "Serbia":
                    return "🇷🇸";

                case "Russia":
                    return "🇷🇺";

                case "Cambodia":
                    return "🇰🇭";

                case "Finland":
                    return "🇫🇮";

                case "Italy":
                    return "🇮🇹";

                default:
                    return "";
            }
        }
    }
}

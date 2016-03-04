namespace BVNetwork.EPiSendMail.Library
{
    public class UtmCode
    {
        /*
            From: https://support.google.com/analytics/answer/1033867?hl=en
        */

        public UtmCode()
        {
            Source = "newsletter";
            Medium = "email";
            Content = "textlink";
        }

        public bool HasValidUtmCode
        {
            get
            {
                // Requires both source, medium, campaign and content
                if(string.IsNullOrEmpty(Source) == false &&
                   string.IsNullOrEmpty(Medium) == false &&
                   string.IsNullOrEmpty(Content) == false &&
                   string.IsNullOrEmpty(Campaign) == false)
                {
                    return true;
                }
                return false;
            } 
        }


        /// <summary>
        /// Campaign Source (utm_source)
        /// Required. Use utm_source to identify a search engine, newsletter name, or other source.
        /// Example: utm_source=google
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Campaign Medium(utm_medium)
        /// Required.Use utm_medium to identify a medium such as email or cost-per- click.
        /// Example: utm_medium=cpc
        /// </summary>
        public string Medium { get; set; }

        /// <summary>
        /// Campaign Name(utm_campaign)
        /// Required.Used for keyword analysis.Use utm_campaign to identify a specific product promotion or strategic campaign.
        /// Example: utm_campaign= spring_sale
        /// </summary>
        public string Campaign { get; set; }

        /// <summary>
        /// Campaign Content(utm_content)
        /// Used for A/B testing and content-targeted ads.Use utm_content to differentiate ads or links that point to the same URL.
        /// Examples: utm_content= logolink or utm_content = textlink
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Not used 
        /// Campaign Term(utm_term)
        /// Used for paid search.Use utm_term to note the keywords for this ad.
        /// Example: utm_term= running + shoes
        /// </summary>
        public string Term { get; set; }


    }
}
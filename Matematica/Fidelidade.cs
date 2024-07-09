using Azure.Monitor.OpenTelemetry.Exporter.Demo.Traces;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matematica
{
    public class Fidelidade
    {
        private ILogger<TraceDemo> logger;

        public Fidelidade(ILogger<TraceDemo> logger)
        {
            this.logger = logger;
        }

        public async Task<Result> ConsultaCampanha(Request request, ActivitySource activitySource)
        {
            logger.Log(LogLevel.Information, "Inicio ConsultaCampanha");
            logger.Log(LogLevel.Debug, "Carrinho:", request);

            IEnumerable<Campaign> campaigns = null;
            var result = new Result();
            using (var getCampaignsActivity = activitySource.StartActivity("getCampaigns", ActivityKind.Client))
            {
                getCampaignsActivity.SetTag("Type", "Database");
                campaigns = await GetCampaings();

            }
            using (var foreachCampaignActivity = activitySource.StartActivity("foreachCampaigns"))
            {
                foreach (var campaign in campaigns)
                {
                    logger.LogDebug($"Iniciando a verificação da campanha {campaign.Id}");
                    using (var campaignActivity = activitySource.StartActivity(ActivityKind.Internal, name: $"campaignVerification:{campaign.Id}"))
                    {
                        try
                        {
                            if (campaign.Rule != null)
                            {
                                FilterByProductRule(request, campaign);
                            }
                            await ApplyDiscount(request.Itens, campaign);
                        }
                        catch (Exception ex)
                        {
                            campaignActivity?.SetStatus(ActivityStatusCode.Error);
                            campaignActivity?.RecordException(ex);
                            logger.LogError(exception: ex, $"Erro ao processar a campanha {campaign.Id}");
                        }
                    }
                    logger.LogDebug($"Fim da verificação da campanha {campaign.Id}");
                }
            }
            logger.Log(LogLevel.Information, "Fim ConsultaCampanha");
            logger.Log(LogLevel.Debug, "Carrinho:", request);
            return result;
        }

        private static void FilterByProductRule(Request request, Campaign campaign)
        {
            var elegibleItens = request.Itens.Where(x => x.Code == campaign.Rule.Code);
        }

        private async Task<bool> ApplyDiscount(IEnumerable<Item> itens, Campaign campaign)
        {
            switch (campaign.DiscountType)
            {
                case "percentage":
                    logger.LogWarning("Operacao lenta no método percentage");
                    await Task.Delay(3000);
                    if (campaign.DiscountBehavior == "allCart")
                        foreach (var item in itens)
                        {
                            item.Price -= (item.Price * (campaign.Value / 100));
                        }
                    break;
                case "discount":
                    itens = null;
                    if (campaign.DiscountBehavior == "allCart")
                    {
                        var total = itens.Sum(x => x.Price);
                        foreach (var item in itens)
                        {
                            var representation = item.Price / total;
                            item.Price = campaign.Value * representation;
                        }
                    }
                    else
                    {
                        itens = itens.OrderByDescending(x => campaign.DiscountBehavior == "onMostExpensive" ? x.Price : x.Price * -1);
                        var remainingValue = campaign.Value;
                        foreach (var item in itens)
                        {
                            if (remainingValue <= 0)
                                break;

                            if (remainingValue <= item.Price)
                            {
                                item.Price -= remainingValue;
                                remainingValue = 0;
                            }
                            else
                            {
                                remainingValue -= item.Price;
                                item.Price = 0;
                            }
                        }
                    }

                    break;
                default:
                    break;
            }
            return await Task.FromResult(true);
        }

        private async Task<IEnumerable<Campaign>> GetCampaings()
        {
            var result = new List<Campaign> { new Campaign { Id = 1, Name = "10% off", DiscountType = "percentage", DiscountBehavior = "allCart", Value = 10 }, new Campaign { Id = 2, Name = "Buy 3 and pay 2", DiscountType = "discount", DiscountBehavior = "OnMostExpesive", Value = 100 } };

            await Task.Delay(2000);
            return await Task.FromResult(result.AsEnumerable());
        }
    }

    public class Campaign
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DiscountType { get; set; }
        public string DiscountBehavior { get; set; }
        public decimal Value { get; set; }
        public Rule Rule { get; set; }
    }

    public class Rule
    {
        public string Code { get; set; }
    }
    public class Request
    {
        public IEnumerable<Item> Itens { get; set; }
    }

    public class Item
    {
        public string Position { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
    public class Result
    {
        public bool Ok { get; set; }
        public string Message { get; set; }
    }
}

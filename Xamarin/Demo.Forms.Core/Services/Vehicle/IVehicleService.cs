
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using ADX365.Forms.Core.Models.Vehicles;
using ADX365.Forms.Core.Models.Vehicles.Auction;
using ADX365.Forms.Core.Models.Vehicles.Transportation;
using ADX365.Forms.Core.Models.Responses;
using ADX365.Forms.Core.Models;

namespace ADX365.Forms.Core.Services.Vehicles
{
    public interface IVehicleService
    {
        Task<CommonResponse> ChangeStatus(VehicleStatus status, int vehicleId);

        /// <summary>
        /// Gets the vehicles list for auction.
        /// </summary>
        /// <returns>The inventory vehicles for auction.</returns>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="inAuction">If set to <c>true</c> in auction.</param>
        /// <param name="minYear">Minimum year.</param>
        /// <param name="maxYear">Max year.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="minMiles">Minimum miles.</param>
        /// <param name="maxMiles">Max miles.</param>
        /// <param name="minPrice">Minimum price.</param>
        /// <param name="maxPrice">Max price.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="ascending">If set to <c>true</c> ascending.</param>
        /// <param name="status">Status.</param>
        /// <param name="auctionStages">Auction stages.</param>
        Task<VehiclesListResponse<Vehicle>> GetInventoryVehiclesForAuction(
            string make
            , string model
            , bool inAuction
            , int minYear
            , int maxYear
            , int pageNumber
            , int minMiles
            , int maxMiles
            , decimal minPrice
            , decimal maxPrice
            , int pageSize
            , string orderBy
            , bool ascending
            , int status
            , string auctionStages
        );

        /// <summary>
        /// Gets the inventory vehicles list for the current dealership.
        /// </summary>
        /// <returns>The inventory vehicles.</returns>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="minYear">Minimum year.</param>
        /// <param name="maxYear">Max year.</param>
        /// <param name="minMiles">Minimum miles.</param>
        /// <param name="maxMiles">Max miles.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="ascending">If set to <c>true</c> ascending.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageNumber">Page number.</param>
        Task<VehiclesListResponse<Vehicle>> GetInventoryVehicles(
            string make
            , string model
            , int minYear
            , int maxYear
            , int minMiles
            , int maxMiles
            , string orderBy
            , bool ascending
            , int pageSize
            , int pageNumber
        );

        /// <summary>
        /// Get the information from an specific vehicle.
        /// </summary>
        /// <returns>The inventory vehicles.</returns>
        /// <param name="vehicleId">Vehicle identifier.</param>
        Task<VehiclesListResponse<Vehicle>> GetInventoryVehiclesForAuction(int vehicleId);

        /// <summary>
        /// Gets the filters for search vehicles list.
        /// </summary>
        /// <param name="inAuction">In auction.</param>
        /// <param name="auctionStages">Auction stages.</param>
        Task<FiltersForAuctionResponse<String>> GetInventoryFiltersForAuction(int inAuction, string auctionStages = null);

        /// <summary>
        /// Gets the filters for the current dealership inventory list.
        /// </summary>
        Task<FiltersForAuctionResponse<string>> GetInventoryFilters();

        Task<modelsResponse<String>> GetModels(string make);

        Task<VehicleOnBidResponse> GetVehicleOnBid(string vin);

        Task<GetVinDescriptionResponse<String>> GetVinDescription(string vin);

        Task<PlaceBidResponse> PlaceBid(decimal amount, int identifier, int BidType);

        Task<PlaceLowerBidResponse<String>> PlaceLowerBid(int identifier, int vehicleId, double amount);

        Task<PlaceBidResponse> PlaceOffer(decimal amount, int auctionId);

        Task<PlaceBidResponse> BuyNow(decimal amount, int BidType, int identifier, int vehicleId);


        /// <summary>
        /// Gets vehicle information related to the relisting process.
        /// </summary>
        Task<GetVehicleInformationResponse<String>> GetVehicleInformation(int vehicleId);

        /// <summary>
        /// Retrieves the lists of parameters required on the Create Vehicle form.
        /// </summary>
        /// <returns>The create vehicle lists.</returns>
        Task<CreateVehicleListsResponse> GetCreateVehicleLists();

        /// <summary>
        /// Gets the vehicle price from the NADA service.
        /// </summary>
        /// <returns>The vehicle price.</returns>
        /// <param name="vin">Vin.</param>
        /// <param name="currentMiles">Current miles.</param>
        /// <param name="trimId">Trim identifier.</param>
        Task<VehiclePriceResponse> GetVehiclePrice(string vin, string currentMiles, int trimId);

        /// <summary>
        /// Gets the available trims for a vehicle VIN according to NADA.
        /// </summary>
        /// <returns>The vehicle price.</returns>
        /// <param name="vin">Vin.</param>
        Task<NADABodyTypesList> GetVehicleTrims(string vin);

        /// <summary>
        /// Gets the basic car information (year-make-model-etc) based on a VIN.
        /// </summary>
        /// <returns>The car information.</returns>
        /// <param name="vin">Vin.</param>
        Task<CarInfoResponse> GetCarInformation(string vin);

        /// <summary>
        /// Creates a vehicle in the BE. 
        /// </summary>
        /// <returns>The vehicle information.</returns>
        /// <param name="completeVehicleInfo">Complete vehicle info.</param>
        Task<CommonResponse> CompleteVehicleInformation(CompleteVehicleRequest completeVehicleInfo);

        /// <summary>
        /// Gets the checkout list.
        /// </summary>
        /// <returns>The checkout list.</returns>
        Task<WonAuctionsResponse> GetCheckoutList(int pDealershipId);

        /// <summary>
        /// Gets the checkout fees.
        /// </summary>
        /// <returns>The checkout fees.</returns>
        Task<List<Fee>> GetCheckoutFees();

        /// <summary>
        /// Gets the detailed vehicle information for a checkout.
        /// </summary>
        /// <returns>The checkout vehicle.</returns>
        /// <param name="pAuctionId">P auction identifier.</param>
        Task<WonVehicle> GetCheckoutVehicle(int pAuctionId);

        /// <summary>
        /// Asks to checkout a vehicle won on an auction.
        /// </summary>
        /// <returns>The checkout.</returns>
        Task<CommonResponse> PostCheckout(CheckoutRequest pCheckoutRequest);

        /// <summary>
        /// Gets a transportation quote from a zip code to another..
        /// </summary>
        Task<TransportationQuote> GetTransportationQuote(int pVehicleId, string pOriginZip, string pDestinationZip);

        /// <summary>
        /// Removes a vehicle from auction.
        /// </summary>
        Task<CommonResponse> RemoveFromAuction(int pVehicleId);

        /// <summary>
        /// Removes a vehicle from the inventory.
        /// </summary>
        Task<CommonResponse> RemoveFromInventory(int pVehicleId);

        /// <summary>
        /// Relists a vehicle in the auction.
        /// </summary>
        Task<CommonResponse> RelistForAuction(int pVehicleId, long pMiles, decimal pReservePrice, decimal pBuyNowPrice, bool pRequiresInspection);
    }
}

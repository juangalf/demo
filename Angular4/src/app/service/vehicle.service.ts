import { vehicle } from "./../model/auction/inventory-response.model";
import { Observable } from "rxjs/Rx";
import { Injectable } from "@angular/core";
import { URLSearchParams } from "@angular/http";

import { HttpHelper } from "@common/http.helper";
import { WebSocketPushService } from "@common/push/web-socket-push.service";
import {
  VehicleDto,
  CreateVehicleListsStatusDto,
  VehicleNADAPriceDto,
  CompleteVehicleInfoDto,
  VehicleModelsModel,
  VehicletitleDto,
  InventoryResponseDto,
  inventoryFiltersDto,
  VehicleListModel,
  VehicleFiltersModel,
  VehicleForAuctionFiltersModel,
  VehicleFullModel,
  VehicleModelFeaturesResponseModel,
  wishlistModelsDto,
  VehicleBiddingHistoryModel,
  WishListDto,
  ListwishlistDto,
  VehiclePreInventoryModel,
  PreInventoryListModel,
  PreInventoryLoadBatchResponseModel,
  QueryEncoderGrades
} from "@model/vehicle.model";
import {
  AuctionBidDescriptionModel,
  AuctionBasicInfoModel
} from "@model/auction.model";
import { StatusModel } from "@model/status.model";
import { AUCTION_PROCESS, AUCTION_STAGE } from "@config/constants.config";

@Injectable()
export class VehicleService {
  private observable: Observable<AuctionBidDescriptionModel>;

  constructor(
    private httpHelper: HttpHelper,
    private pushService: WebSocketPushService
  ) {}
  ws: WebSocket;
  /**
   * Subscribes to the push service to recieve information of an auction.
   * @param auctionId
   */
  watchAuctionDetails(
    auctionId: number
  ): Observable<AuctionBidDescriptionModel> {
    this.observable = this.pushService.listen(
      "AuctionBidDescriptionDto",
      auctionId
    );
    this.ws = this.pushService.ws;
    return this.observable;
  }

  /**
   * Subscribes to the push service to recieve information of all the current auctions.
   */
  watchVehicleSearch(): Observable<AuctionBasicInfoModel> {
    return this.pushService.listen("MinimalAuctionInfoDto", 0);
  }

  /**
   * The service to bring the list of vehicles based on the filters.
   *
   * @param inAuction
   * @param minYear
   * @param maxYear
   * @param pageNumber
   * @param pageSize
   * @param minMiles
   * @param maxMiles
   * @param minPrice
   * @param maxPrice
   * @param orderBy
   * @param ascending
   * @param make
   * @param model
   */
  getVehiclesToSearch(
    inAuction: number,
    minYear: number,
    maxYear: number,
    pageNumber: number,
    pageSize: number,
    minMiles: number,
    maxMiles: number,
    minPrice: number,
    maxPrice: number,
    orderBy: string,
    ascending: boolean,
    make?: string,
    model?: string,
    dealershipId?: number,
    adxGrades?: string,
    bodyTypes?: string,
    doors?: string,
    driveTrains?: string,
    engineCylinders?: string,
    exteriorColors?: string,
    fuels?: string,
    interiorColors?: string,
    modelsYears?: string,
    numbersOfKeys?: string,
    transmissions?: string,
    auctionProcess: number = 0
  ): Observable<VehicleListModel> {
    // Parameter format.
    let parameters = new URLSearchParams("", new QueryEncoderGrades());
    if (inAuction === 1) {
      parameters.set("inAuction", "true");
    } else if (inAuction === 2) {
      parameters.set("inAuction", "false");
    }
    if (minYear) parameters.set("minYear", minYear.toString());
    if (maxYear) parameters.set("maxYear", maxYear.toString());
    parameters.set("pageNumber", pageNumber.toString());
    if (minMiles) parameters.set("minMiles", minMiles.toString());
    if (maxMiles) parameters.set("maxMiles", maxMiles.toString());
    if (minPrice) parameters.set("minPrice", minPrice.toString());
    if (maxPrice) parameters.set("maxPrice", maxPrice.toString());
    parameters.set("pageSize", pageSize.toString());
    parameters.set("orderBy", orderBy);
    parameters.set("ascending", ascending.toString());
    parameters.set("status", inAuction.toString());
    if (make) parameters.set("make", make);
    if (model) parameters.set("model", model);
    if (adxGrades) parameters.set("adxGrades", adxGrades);
    if (bodyTypes) parameters.set("bodyTypes", bodyTypes);
    if (doors) parameters.set("doors", doors);
    if (driveTrains) parameters.set("driveTrains", driveTrains);
    if (engineCylinders) parameters.set("engineCylinders", engineCylinders);
    if (exteriorColors) parameters.set("exteriorColors", exteriorColors);
    if (fuels) parameters.set("fuels", fuels);
    if (interiorColors) parameters.set("interiorColors", interiorColors);
    if (modelsYears) parameters.set("modelsYears", modelsYears);
    if (numbersOfKeys) parameters.set("numbersOfKeys", numbersOfKeys);
    if (transmissions) parameters.set("transmissions", transmissions);
    if (dealershipId) parameters.set("dealershipId", dealershipId.toString());

    if (auctionProcess == AUCTION_PROCESS.ON_DEMAND) {
      parameters.set(
        "auctionStages",
        AUCTION_STAGE.STAGE1 + "," + AUCTION_STAGE.STAGE2
      );
    }
    if (auctionProcess == AUCTION_PROCESS.MAKE_A_DEAL) {
      parameters.set("auctionStages", "" + AUCTION_STAGE.STAGE3);
    }
    // Call the API.
    return this.httpHelper.getWithUrlParams("get-vehicles", parameters);
  }

  /**
   * Retrieves the filter values for the search vehicle screens.
   * @param inAuction
   */
  getVehiclesToSearchFilters(
    inAuction: number,
    auctionProcess: number = 0
  ): Observable<VehicleFiltersModel> {
    let parameters = new URLSearchParams();
    parameters.set("inAuction", inAuction.toString());
    if (auctionProcess == AUCTION_PROCESS.ON_DEMAND) {
      parameters.set(
        "auctionStages",
        AUCTION_STAGE.STAGE1 + "," + AUCTION_STAGE.STAGE2
      );
    }
    if (auctionProcess == AUCTION_PROCESS.MAKE_A_DEAL) {
      parameters.set("auctionStages", "" + AUCTION_STAGE.STAGE3);
    }
    return this.httpHelper.getWithUrlParams("get-vehicle-filters", parameters);
  }

  getVehicleToAutionFilters(
    minYear: number = 1990,
    maxYear: number = new Date().getFullYear(),
    maxMiles: number,
    minPrice: number,
    maxPrice: number,
    make: string,
    model: string,
    inAuction: boolean = true,
    auctionProcess: number = 0
  ): Observable<VehicleForAuctionFiltersModel> {
    let parameters = new URLSearchParams();
    parameters.set("inAuction", inAuction.toString());
    if (make) parameters.set("make", make);
    if (model) parameters.set("model", model);
    if (minYear) parameters.set("minYear", minYear.toString());
    if (maxYear) parameters.set("maxYear", maxYear.toString());
    if (maxMiles) parameters.set("maxMiles", maxMiles.toString());
    if (minPrice) parameters.set("minPrice", minPrice.toString());
    if (minPrice) parameters.set("maxPrice", maxPrice.toString());
    if (auctionProcess == AUCTION_PROCESS.ON_DEMAND) {
      parameters.set(
        "auctionStages",
        AUCTION_STAGE.STAGE1 + "," + AUCTION_STAGE.STAGE2
      );
    }
    if (auctionProcess == AUCTION_PROCESS.MAKE_A_DEAL) {
      parameters.set("auctionStages", "" + AUCTION_STAGE.STAGE3);
    }

    return this.httpHelper.getWithUrlParams(
      "get-vehicle-auction-filters",
      parameters
    );
  }

  /**
   * Returns the autocheck information for a VIN.
   * @param vin
   */
  getAutocheckInformationByVin(pVin: string): Observable<any> {
    // Set the parameters.
    let parameters = new URLSearchParams();
    parameters.set("vin", pVin);
    // Set a responsive user agent.
    let userAgent = "iPhone";
    parameters.set("userAgent", userAgent);
    // Call the service.
    return this.httpHelper.getWithUrlParams(
      "get-autocheck-information",
      parameters
    );
  }

  /**
   * Retrieves a full detailed model for a vehicle from the server.
   * @param vin
   */
  getVehicleFullDetails(vin: string): Observable<VehicleFullModel> {
    let parameters = new URLSearchParams();
    parameters.set("vin", vin);
    return this.httpHelper.getWithUrlParams(
      "get-vehicle-full-details",
      parameters
    );
  }

  /**
   * Retrieves the model features and specifications for a VIN.
   * @param vin
   */
  getVehicleModelFeatures(
    vin: string
  ): Observable<VehicleModelFeaturesResponseModel> {
    let parameters = new URLSearchParams();
    parameters.set("vin", vin);
    return this.httpHelper.getWithUrlParams(
      "get-vehicle-model-features",
      parameters
    );
  }

  /**
   * Retrieves the bidding history of a vehicle based on its ID.
   * @param vin
   */
  getBiddingHistory(vehicleId: number): Observable<VehicleBiddingHistoryModel> {
    let parameters = new URLSearchParams();
    parameters.set("vehicleId", vehicleId.toString());
    return this.httpHelper.getWithUrlParams(
      "get-vehicle-bidding-history",
      parameters
    );
  }

  /**
   * Retrieves the models based on a make.
   * @param make
   */
  getModelsByMake(make: string): Observable<VehicleModelsModel> {
    let parameters = new URLSearchParams();
    parameters.set("make", make);
    return this.httpHelper.getWithUrlParams("get-models-by-make", parameters);
  }

  getAutocheckInformation(vinnumber: string): Observable<any> {
    // Set the parameters.
    let vinParams = "?vin=" + vinnumber;

    // Call the service.
    return this.httpHelper.getWithParams(
      "get-autocheck-information",
      vinParams
    );
  }

  getCreateVehicleLists(): Observable<CreateVehicleListsStatusDto> {
    return this.httpHelper.get("get-create-vehicle-lists");
  }

  getCarInformation(vinnumber: string): Observable<VehicleDto> {
    let vinParams = "?vin=" + vinnumber;
    return this.httpHelper.getWithParams("get-car-information", vinParams);
  }

  getVehiclePrice(
    vin: string,
    currentMiles: number,
    trim?: number
  ): Observable<VehicleNADAPriceDto> {
    let parameters = new URLSearchParams();
    parameters.set("vin", vin);
    parameters.set("CurrentMiles", currentMiles.toString());
    parameters.set("trim", trim ? trim.toString() : null);
    return this.httpHelper.getWithUrlParams("get-NADA-prices", parameters);
  }

  getSoldVehicle(dealershipId: number): Observable<any> {
    let parameters = new URLSearchParams();
    parameters.set("minYear", "1900");
    parameters.set("maxYear", "2999");
    parameters.set("pageNumber", "1");
    parameters.set("minMiles", "0");
    parameters.set("maxMiles", "9999999");
    parameters.set("pageSize", "100");
    parameters.set("orderBy", "miles");
    parameters.set("ascending", "true");
    parameters.set("status", "7");

    return this.httpHelper.getWithUrlParams("get-sold-vehicle", parameters);
  }

  postVehicleInfo(info: CompleteVehicleInfoDto): Observable<StatusModel> {
    return this.httpHelper.post("complete-vehicle-info", info);
  }

  getVehicleContainsVin(vinnumber: string): Observable<InventoryResponseDto> {
    let vinParams = "?vin=" + vinnumber;
    return this.httpHelper.getWithParams("get-vin-vehicle", vinParams);
  }

  // createVehicleTitle(vehicletitle: VehicletitleDto): Observable<InventoryResponseDto> {
  //   return this.httpHelper.post('create-vehicletitle', vehicletitle);
  // }

  createVehicleTitle(formdata: any): Observable<InventoryResponseDto> {
    return this.httpHelper.postMultiPart<FormData, InventoryResponseDto>(
      "create-vehicletitle",
      formdata
    );
  }

  updateVehicleTrim(vehicleId: number, trim: number) {
    let parameters = new URLSearchParams();
    parameters.set("vehicleId", vehicleId.toString());
    parameters.set("trim", trim.toString());
    return this.httpHelper.putWithUrlParams(
      "update-vehicle-trim",
      parameters,
      {}
    );
  }

  getInventoryFilters(): Observable<inventoryFiltersDto> {
    return this.httpHelper.get("get-inventory-filters");
  }

  getMyInventory(
    status: number,
    minYear: number,
    maxYear: number,
    pageNumber: number,
    pageSize: number,
    minMiles: number,
    maxMiles: number,
    orderBy: string,
    ascending: boolean,
    make?: string,
    model?: string,
    includeChildren?: boolean,
    dealershipId?: number
  ): Observable<InventoryResponseDto> {
    let parameters = new URLSearchParams();
    parameters.set("minYear", minYear.toString());
    parameters.set("maxYear", maxYear.toString());
    parameters.set("pageNumber", pageNumber.toString());
    parameters.set("minMiles", minMiles.toString());
    parameters.set("maxMiles", maxMiles.toString());
    parameters.set("pageSize", pageSize.toString());
    parameters.set("orderBy", orderBy);
    parameters.set("ascending", ascending.toString());
    // Some codification required for the status.
    let statusParameter = "";
    if (status !== 0) {
      statusParameter = status.toString();
    }
    parameters.set("status", statusParameter);
    if (make) parameters.set("make", make);
    if (model) parameters.set("model", model);
    parameters.set("includeChildren", includeChildren.toString()); //aqui es true o false

    if (dealershipId > 0) {
      parameters.set("dealershipId", dealershipId.toString());
    }

    return this.httpHelper.getWithUrlParams("get-my-inventory", parameters);
  }

  getMyInventoryByVin(
    vin: string,
    includeChildren: boolean
  ): Observable<InventoryResponseDto> {
    let parameters = new URLSearchParams();
    parameters.set("vin", vin);
    parameters.set("includeChildren", includeChildren.toString()); //aqui es true o false
    return this.httpHelper.getWithUrlParams("get-my-inventory", parameters);
  }

  getWishList(dealershipId: number): Observable<ListwishlistDto> {
    let parameters = new URLSearchParams();
    parameters.set("DealershipId", dealershipId.toString());
    return this.httpHelper.getWithUrlParams("get-wishlist", parameters);
  }

  getMakeList(year: number) {
    let parameters = new URLSearchParams();
    parameters.set("modelYear", year.toString());
    return this.httpHelper.getWithUrlParams("get-makelist", parameters);
  }

  getModelByMakeAndYear(modelYear: number, make: string) {
    let parameters = new URLSearchParams();
    parameters.set("modelYear", modelYear.toString());
    parameters.set("make", make);
    return this.httpHelper.getWithUrlParams("get-modellist", parameters);
  }

  saveWishList(wishListDto: WishListDto): Observable<WishListDto> {
    return this.httpHelper.post("create-wishlist", wishListDto);
  }

  updateWishList(wishListDto: WishListDto): Observable<WishListDto> {
    return this.httpHelper.post("update-wishlist", wishListDto);
  }

  deleteWishListItem(wishListDto: WishListDto) {
    //let parameters = new URLSearchParams();
    //parameters.set('id', id.toString());
    return this.httpHelper.delete("delete-wishlistitem", wishListDto);
  }

  /**
   * Creates a vehicle in the pre-inventory.
   * @param newVehicle
   */
  createPreInventoryVehicle(
    newVehicle: VehiclePreInventoryModel
  ): Observable<PreInventoryListModel> {
    return this.httpHelper.post("create-pre-inventory-vehicle", newVehicle);
  }

  /**
   * Creates a list of vehicles in the pre-inventory.
   * @param newVehicles
   */
  createPreInventoryVehiclesBatch(
    dealershipId: number,
    newVehicles: any
  ): Observable<PreInventoryLoadBatchResponseModel> {
    // Create the body of the message.
    let multipartFormData = new FormData();
    multipartFormData.append("Dto", JSON.stringify({ id: dealershipId }));
    multipartFormData.append("PreInventoryFile", newVehicles, newVehicles.name);
    // Send.
    return this.httpHelper.postMultiPart<
      FormData,
      PreInventoryLoadBatchResponseModel
    >("create-pre-inventory-vehicles-batch", multipartFormData);
  }

  /**
   * Retrieves the pre-inventory list.
   * @param newVehicle
   */
  getPreInventory(dealershipId: number): Observable<PreInventoryListModel> {
    let parameters = new URLSearchParams();
    parameters.set("dealershipId", dealershipId.toString());
    return this.httpHelper.getWithUrlParams("get-pre-inventory", parameters);
  }

  /**
   * Deletes a vehicle from the pre-inventory list.
   * @param newVehicle
   */
  deletePreInventoryVehicle(
    id: number,
    dealershipId: number
  ): Observable<PreInventoryListModel> {
    return this.httpHelper.delete("delete-pre-inventory-vehicle", {
      id: id,
      dealershipId: dealershipId
    });
  }

  getCloseToList(): Observable<any> {
    return this.httpHelper.get("get-sold-closed-vehicles");
  }

  UpdateVehiclePaymentOption(vehicleId: number, paymentInfo: string) {
    let parameters = new URLSearchParams();
    parameters.set("vehicleId", vehicleId.toString());
    parameters.set("paymentInfo", paymentInfo);
    return this.httpHelper.putWithUrlParams(
      "update-vehicle-payment-option",
      parameters,
      {}
    );
  }

  getNADABodyTypes(vin: string) {
    let parameters = new URLSearchParams();
    parameters.set("vin", vin);
    return this.httpHelper.getWithUrlParams("get-NADA-body-types", parameters);
  }

  GetTransportPrices(
    vehicleId,
    originZipCode,
    destinationZipCode,
    openTrailerType?: boolean
  ): Observable<any> {
    let parameters = new URLSearchParams();
    parameters.set("vehicleId", vehicleId.toString());
    parameters.set("originZipCode", originZipCode.toString());
    parameters.set("destinationZipCode", destinationZipCode.toString());

    if (typeof openTrailerType != "undefined") {
      parameters.set("openTrailerType", openTrailerType.toString());
    }

    return this.httpHelper.getWithUrlParams(
      "get-transport-prices",
      parameters
    );
  }
}

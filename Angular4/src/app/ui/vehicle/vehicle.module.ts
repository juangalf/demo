import { DealerService } from './../../service/dealer.service';
import { NgModule } from "@angular/core";
import { CommonModule, CurrencyPipe } from "@angular/common";
import { RouterModule } from "@angular/router";
import { HttpClientModule } from "@angular/common/http";
import { FormsModule, ReactiveFormsModule, NgModel } from "@angular/forms";

import { VehicleRoutingModule } from "./vehicle-routing.module";
import { VehicleSearchComponent } from "./search/vehicle-search.component";
import { VehicleCreateComponent } from "./create/vehicle-create.component";
import { VehicleAutocheckComponent } from "./autocheck/vehicle-autocheck.component";
import { VehicleDetailComponent } from "./detail/vehicle-detail.component";
import { VehicleModelFeaturesComponent } from "./detail/vehicle-model-features.component";
import { VehicleBiddingHistoryComponent } from "./detail/vehicle-bidding-history.component";
import { NadaButton } from "./components/nada-button.component";
import { VehicleTitleComponent } from "./title/vehicle-title.component";
import { VehiclePreinventory } from "./preinventory/vehicle-preinventory.component";
import { VehiclePreinventoryCreate } from "./preinventory/vehicle-preinventory-create.component";

import { DateParserUtil } from "@common/util/date-parser.util";
import { NumberMaskUtil } from "@common/util/number-mask.util";
import { ImageZoom } from "@common/directive/image-zoom/image-zoom.directive";
import { ImageZoomContainer } from "@common/directive/image-zoom/image-zoom-container.component";
import { ImageZoomLens } from "@common/directive/image-zoom/image-zoom-lens.component";
import { DealershipService } from "@service/dealership.service";
import { VehicleManager } from "@business/vehicle/vehicle.manager";
import { VehicleService } from "@service/vehicle.service";
import { AuctionService } from "@service/auction.service";
import { GatePassService} from "@service/gate-pass.service";

import { UiModule } from "../ui.module";
import { BaseVehicleListComponent } from "@common/component/base/base-vehicle-list.component";
import { AutocheckHtml } from "@common/class/autocheck-html";
import { VehicleTitlePriceComponent } from "app/ui/vehicle/create/vehicle-title-price.component";
import { VehicleDetailsComponent } from "app/ui/vehicle/create/vehicle-details.component";
import { InventoryManager } from "@business/inventory.manager";
import { InventoryService } from "@service/inventory.service";
import { AuctionManager } from "@business/auction/auction.manager";
import { ItemVehicleComponent } from "./detail/item-vehicle.component";
import { VehicleImagesComponent } from "./detail/vehicle-images.component";
import { VehicleDescriptionComponent } from "./detail/vehicle-description.component";

import {
  BaseSearchComponent,
  OnDemandSearchComponent,
  MakeADealSearchComponent
} from "./home/base-search.component";
import { MainFilterComponent } from "./home/main-filter.component";
import { NarrowFilterComponent } from "./home/narrow-filter.component";
import { ItemSearchComponent } from "./home/item-search.component";
import { CounterOfferService } from "@service/counter-offer.service";
import { ParameterService } from "@service/parameter.service";
import { InfiniteScrollModule } from "ngx-infinite-scroll";
import { SoldVehiclesComponent } from "./sold-vehicles/sold-vehicles.component";
import { CloseToSoldComponent } from "./close-to-sold/close-to-sold.component";
import { TrimUpdateComponent } from "./trim-update/trim-update.component";

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    VehicleRoutingModule,
    InfiniteScrollModule,
    UiModule.forRoot()
  ],
  declarations: [
    VehicleSearchComponent,
    VehicleDetailComponent,
    VehicleModelFeaturesComponent,
    VehicleBiddingHistoryComponent,
    VehicleCreateComponent,
    VehicleAutocheckComponent,
    VehicleTitleComponent,
    NadaButton,
    BaseVehicleListComponent,
    ImageZoom,
    ImageZoomContainer,
    ImageZoomLens,
    VehiclePreinventory,
    VehiclePreinventoryCreate,
    VehicleTitlePriceComponent,
    VehicleDetailsComponent,
    ItemVehicleComponent,
    VehicleImagesComponent,
    VehicleDescriptionComponent,
    SoldVehiclesComponent,
    OnDemandSearchComponent,
    MakeADealSearchComponent,
    BaseSearchComponent,
    MainFilterComponent,
    NarrowFilterComponent,
    ItemSearchComponent,
    CloseToSoldComponent,
    TrimUpdateComponent
  ],
  exports: [],
  providers: [
    VehicleService,
    CurrencyPipe,
    VehicleManager,
    DealershipService,
    AuctionService,
    DateParserUtil,
    NumberMaskUtil,
    AutocheckHtml,
    InventoryManager,
    InventoryService,
    AuctionManager,
    CounterOfferService,
    ParameterService,
    GatePassService,
    DealerService
  ],
  entryComponents: [ImageZoomContainer, ImageZoomLens]
})
export class VehicleModule {}

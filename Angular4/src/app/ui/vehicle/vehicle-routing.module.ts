import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VehicleSearchComponent } from './search/vehicle-search.component';
import { VehicleCreateComponent } from './create/vehicle-create.component';
import { VehicleAutocheckComponent } from './autocheck/vehicle-autocheck.component';
import { VehicleDetailComponent } from './detail/vehicle-detail.component';
import { VehicleTitleComponent } from './title/vehicle-title.component';
import { VehiclePreinventory } from './preinventory/vehicle-preinventory.component';
import { ItemVehicleComponent } from './detail/item-vehicle.component';
import { OnDemandSearchComponent, MakeADealSearchComponent } from './home/base-search.component';
import { SoldVehiclesComponent } from './sold-vehicles/sold-vehicles.component';
import { CloseToSoldComponent } from "./close-to-sold/close-to-sold.component";
import { TrimUpdateComponent } from './trim-update/trim-update.component';

const routes: Routes = [
    //{ path: '', component: VehicleSearchComponent },
    { path: '', component: OnDemandSearchComponent },
    { path: 'by-dealership/:dealershipId', component: VehicleSearchComponent },
    { path: 'addvehicle', component: VehicleCreateComponent },
    { path: 'update', component: VehicleCreateComponent },
    { path: 'details/:vin', component: VehicleCreateComponent },
    { path: 'autocheck/:vin', component: VehicleAutocheckComponent },
    { path: 'detail', component: VehicleDetailComponent },
    { path: 'title', component: VehicleTitleComponent },
    { path: 'preinventory', component: VehiclePreinventory },
    { path: 'on-demand', component: OnDemandSearchComponent },
    { path: 'offer-buy-now', component: MakeADealSearchComponent },
    { path: 'sold-vehicles', component: SoldVehiclesComponent },
    { path: 'close-to-sold', component: CloseToSoldComponent },
    { path: 'trim', component: TrimUpdateComponent },
    { path: ':vehicleId', component: ItemVehicleComponent },

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class VehicleRoutingModule { }

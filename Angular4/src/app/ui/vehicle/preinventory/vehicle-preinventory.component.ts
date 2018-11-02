import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { MESSAGES, MODAL, ACTION } from '@config/config';

import { SecurityManager } from '@business/core/security.manager';

import { PreInventoryListModel } from '@model/vehicle.model';
import { VehicleService } from '@service/vehicle.service';
import { BaseComponent } from '@common/component/base/base.component';
import { ModalService } from '@common/component/modal/modal.service';
import { GUARD } from '@config/guard.config';


@Component({
    selector: 'vehicle-preinventory'
    , templateUrl: './vehicle-preinventory.component.html'
    , styleUrls: ['./vehicle-preinventory.component.css']
})
export class VehiclePreinventory extends BaseComponent {

    /**
     * The current dealership.
     */
    dealershipId: number;
    /**
     * The response from the server.
     */
    serviceResponse: PreInventoryListModel;

    /**
     *
     * @param vehicleService
     * @param router
     * @param modal
     */
    constructor(
        protected vehicleService: VehicleService
        , protected router: Router
        , public modal: ModalService
        , protected securityManager: SecurityManager
    ) {
        super(modal, router, securityManager);
        this.checkGuard(GUARD.preInventory);
    }

    /**
     * Init
     */
    ngOnInit() {
        this.dealershipId = this.securityManager.getDealershipId();
        this.getPreInventory();
    }


    /**
     * Calls the service to bring the list of cars in the pre-inventory. Populates the model.
     */
    getPreInventory(): void {
        this.modal.showLoading();
        // Call the service.
        this.vehicleService.getPreInventory(this.dealershipId).subscribe(
            response => {
                this.modal.dismiss();
                this.serviceResponse = response;
            },
            er => {
                this.modal.dismiss();
                this.onError(er);
            },
            () => {
                if (!this.serviceResponse.status) {
                    this.onErrorSilent(this.serviceResponse, MESSAGES.vehiclePreInventory);
                }
            }
        );
    }

    /**
     * Triggered when we receive a successful creation event from the create component.
     * @param creationResponse
     */
    onCreated(creationResponse: PreInventoryListModel): void {
        this.serviceResponse = creationResponse;
    }

    /**
     * The user clicked on add to inventory.
     */
    addToInventoryClick(vehicle): void {
        this.router.navigate(['vehicle/addvehicle'], { queryParams: { vehicle: JSON.stringify(vehicle) } });
    }

    /**
     * The user clicked on delete.
     */
    deleteClick(id: number, year: number, make: string, model: string): void {
        let vehicleText = "";
        if ( year || make || model ){
            vehicleText = " " + year + " " + make + " " + model;
        }
        else {
            vehicleText = " vehicle";
        }
        let confirmationMessage = MESSAGES.vehiclePreInventory[200] + vehicleText + "?";
        this.modal.showConfirm({ text: confirmationMessage, type: MODAL.warning }).subscribe(
            rs => {
                if (rs.action == ACTION.YES) {
                    this.modal.showLoading();
                    // Call the service.
                    this.vehicleService.deletePreInventoryVehicle(id, this.dealershipId).subscribe(
                        response => {
                            this.modal.dismiss();
                            this.serviceResponse = response;
                        },
                        er => {
                            this.modal.dismiss();
                            this.onError(er);
                        },
                        () => {
                            if (this.serviceResponse.status) {
                                this.modal.showSuccess(MESSAGES.vehiclePreInventory[100]);
                            }
                            else {
                                this.onError(this.serviceResponse, MESSAGES.vehiclePreInventory);
                            }
                        }
                    );
                }
            }
        );
    }
}

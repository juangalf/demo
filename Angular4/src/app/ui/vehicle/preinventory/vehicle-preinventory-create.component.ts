import { Component, OnInit, EventEmitter, Output } from "@angular/core";
import { Router } from "@angular/router";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

import { MESSAGES } from "@config/config";

import { NumberMaskUtil } from "@common/util/number-mask.util";

import { SecurityManager } from "@business/core/security.manager";

import { StatusModel } from "@model/status.model";
import {
  VehiclePreInventoryModel,
  PreInventoryListModel,
  PreInventoryLoadBatchResponseModel
} from "@model/vehicle.model";

import { VehicleService } from "@service/vehicle.service";
import { VehicleManager } from "@business/vehicle/vehicle.manager";
import { VehicleNADAPriceDto } from "@model/vehicle.model";
import { BaseComponent } from "@common/component/base/base.component";
import { ModalService } from "@common/component/modal/modal.service";
import { GUARD } from "@config/guard.config";

@Component({
  selector: "vehicle-preinventory-create",
  templateUrl: "./vehicle-preinventory-create.component.html",
  styleUrls: ["./vehicle-preinventory-create.component.css"]
})
export class VehiclePreinventoryCreate extends BaseComponent {
  /**
   * Output
   */
  @Output() onCreated = new EventEmitter<PreInventoryListModel>();
  /**
   * Constants
   */
  INPUT_FORM_MILES: string = "currentMiles";
  INPUT_FORM_PRICE: string = "price";
  /**
   *
   */
  model: VehiclePreInventoryModel = new VehiclePreInventoryModel();
  /**
   *
   */
  vehiclesFile: any;
  /**
   *
   */
  submitted = false;
  /**
   * The response from the server.
   */
  serviceResponse: PreInventoryListModel;
  /**
   * The response from the server for the batch load.
   */
  batchServiceResponse: PreInventoryLoadBatchResponseModel;
  /**
   *
   */
  form: FormGroup;
  NADAProccesed: boolean = false;
  vehicleNADAPrice: VehicleNADAPriceDto;

  /**
   *
   * @param vehicleService
   * @param router
   * @param modal
   */
  constructor(
    protected vehicleService: VehicleService,
    protected router: Router,
    public modal: ModalService,
    protected securityManager: SecurityManager,
    private formBuilder: FormBuilder,
    protected numberMask: NumberMaskUtil,
    private manager: VehicleManager
  ) {
    super(modal, router, securityManager);
    this.checkGuard(GUARD.preInventory);
  }

  /**
   * Init
   */
  ngOnInit() {
    this.model.dealershipid = this.securityManager.getDealershipId();
    this.initForm();
  }

  /**
   * Init form.
   */
  initForm(): void {
    this.form = this.formBuilder.group({
      vin: [
        "",
        Validators.compose([Validators.maxLength(50), Validators.required])
      ],
      currentMiles: [
        "",
        Validators.compose([Validators.maxLength(50), Validators.required])
      ],
      price: [
        "",
        Validators.compose([Validators.maxLength(50), Validators.required])
      ]
    });
  }

  /**
   * Called when the form is submitted.
   */
  onSubmit() {
    this.modal.showLoading();
    // Transform.
    let newVehicle = this.getVehicleToSend();
    // Call the service.
    this.vehicleService.createPreInventoryVehicle(newVehicle).subscribe(
      response => {
        this.modal.dismiss();
        this.serviceResponse = response;
      },
      er => {
        this.modal.dismiss();
        this.onError(er);
      },
      () => {
        this.serviceResponse.status
          ? this.onHandleSuccessfulCreation(true)
          : this.onError(this.serviceResponse, MESSAGES.vehiclePreInventory);
      }
    );
    this.submitted = true;
  }

  /**
   * Prepoares the data of the object to be sent to the server.
   */
  getVehicleToSend(): VehiclePreInventoryModel {
    let newVehicle = new VehiclePreInventoryModel();
    newVehicle.currentMiles = Number.parseInt(
      this.numberMask.unmask(this.model.currentMiles.toString())
    );
    newVehicle.dealershipid = this.model.dealershipid;
    newVehicle.vin = this.model.vin;
    newVehicle.price = this.model.price;
    return newVehicle;
  }

  /**
   * Called when the creation was successfull.
   */
  onHandleSuccessfulCreation(mainSubmit: boolean): void {
    // Emit the response.
    this.onCreated.emit(this.serviceResponse);
    // Message.
    let message = MESSAGES.vehiclePreInventory[0];
    if (!mainSubmit) {
      message = MESSAGES.vehiclePreInventory[10];
    }
    this.modal.showSuccess(message);
  }

  /**
   * When the user enter something in miles.
   * @param event
   */
  onMilesKeyEvt(value: string): void {
    // Get the raw number.
    let nStr: string = this.numberMask.unmask(value);
    if (nStr) {
      // Mask
      let milesMask = this.numberMask.mask(nStr, 7);
      if (value !== milesMask) {
        this.form.get(this.INPUT_FORM_MILES).setValue(milesMask);
      }
    } else if (value) {
      this.form.get(this.INPUT_FORM_MILES).setValue("");
    }
  }

  /**
   * When the user enter something in price.
   * @param event
   */
  onPriceKeyEvt(value: string): void {
    // Get the raw number.
    let nStr: string = this.numberMask.unmask(value);
    if (nStr) {
      // Mask
      let priceMask = this.numberMask.mask(nStr, 7);
      if (value !== priceMask) {
        this.form.get(this.INPUT_FORM_PRICE).setValue(priceMask);
      }
    } else if (value) {
      this.form.get(this.INPUT_FORM_PRICE).setValue("");
    }
  }

  /**
   * Triggered when the user selects a file.
   * @param event
   */
  onFileSelect(event) {
    if (event.file.status == 0) {
      this.vehiclesFile = event.file.files[0];
    }
  }

  /**
   * The user clicks on the submit file button.
   */
  submitFile(): void {
    this.modal.showLoading();
    // Call the service.
    this.vehicleService
      .createPreInventoryVehiclesBatch(
        this.model.dealershipid,
        this.vehiclesFile
      )
      .subscribe(
        response => {
          this.modal.dismiss();
          this.batchServiceResponse = response;
        },
        er => {
          this.modal.dismiss();
          this.onError(er);
        },
        () => {
          this.batchServiceResponse.status
            ? this.onHandleSuccessfulBatchCreation()
            : this.onError(
                this.batchServiceResponse,
                MESSAGES.vehiclePreInventory
              );
        }
      );
  }

  /**
   * Called when the creation was successfull.
   */
  onHandleSuccessfulBatchCreation(): void {
    // Create the object to emit.
    let emitObject = new PreInventoryListModel();
    emitObject.listpreinventory = this.batchServiceResponse.result;
    emitObject.status = this.batchServiceResponse.status;
    emitObject.messageCode = this.batchServiceResponse.messageCode;

    // Emit the response.
    this.onCreated.emit(emitObject);

    // Message.
    this.modal.show("batch-result-modal");
  }

  showTradeInValues(): void {
    this.manager
      .getVehiclePrice(this.model.vin, this.model.currentMiles,null) //null value for trim
      .subscribe(
        rs => {
          if (rs.status) {
            this.vehicleNADAPrice = rs;
          } else {
            if (rs.messageCode == 99) {
              this.modal.showWarning(
                "NADA can not provide a price for this vehicle. We can not add the vehicle at this time."
              );
            }
          }
        },
        er => {
          //console.log(er);
        },
        () => {
          this.NADAProccesed = true;
        }
      );
  }

  getNumberRounded50(value) {
    let number_rounded = 1 * value;
    const number_module = value % 50;
    if (number_module != 0) {
      number_rounded = value + (50 - number_module);
    }

    return number_rounded;
  }

  removeDecimals(value) {
    let new_value = value;

    if (Number(value) != NaN) {
      new_value = Number(Number(value).toFixed(0));
    }
    return new_value;
  }

  NADAPriceValidation(modelPrice) {
    let invalid = false;

    const numberModelPrice = Number(modelPrice.replace(/,/g, ""));

    if (
      numberModelPrice >
        this.getNumberRounded50(this.vehicleNADAPrice.avgtradein) ||
      numberModelPrice <
        this.getNumberRounded50(this.vehicleNADAPrice.roughtradein)
    ) {
      invalid = true;
    }

    return invalid;
  }
}

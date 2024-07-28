using System;
using System.Collections.Generic;

namespace CK.Models;

public partial class Salesline
{
    public string Activitynumber { get; set; } = null!;

    public long Addressrefrecid { get; set; }

    public int Addressreftableid { get; set; }

    public int Agreementskipautolink { get; set; }

    public string AssetidRu { get; set; } = null!;

    public string Barcode { get; set; } = null!;

    public string Barcodetype { get; set; } = null!;

    public int Blocked { get; set; }

    public int Casetagging { get; set; }

    public int Complete { get; set; }

    public DateTime Confirmeddlv { get; set; }

    public decimal Costprice { get; set; }

    public string CountryregionnameRu { get; set; } = null!;

    public string Countyorigdest { get; set; } = null!;

    public long CreditnoteinternalrefPl { get; set; }

    public long Creditnotereasoncode { get; set; }

    public string Currencycode { get; set; } = null!;

    public string Custaccount { get; set; } = null!;

    public string Custgroup { get; set; } = null!;

    public int Customerlinenum { get; set; }

    public string Customerref { get; set; } = null!;

    public DateTime CustomsdocdateMx { get; set; }

    public string CustomsdocnumberMx { get; set; } = null!;

    public string CustomsnameMx { get; set; } = null!;

    public long Defaultdimension { get; set; }

    public int Deliverydatecontroltype { get; set; }

    public string Deliveryname { get; set; } = null!;

    public long Deliverypostaladdress { get; set; }

    public string DeliverytaxgroupBr { get; set; } = null!;

    public string DeliverytaxitemgroupBr { get; set; } = null!;

    public int Deliverytype { get; set; }

    public string Dlvmode { get; set; } = null!;

    public string Dlvterm { get; set; } = null!;

    public string Einvoiceaccountcode { get; set; } = null!;

    public decimal Expectedretqty { get; set; }

    public string Externalitemid { get; set; } = null!;

    public string Intercompanyinventtransid { get; set; } = null!;

    public int Intercompanyorigin { get; set; }

    public DateTime IntrastatfulfillmentdateHu { get; set; }

    public decimal Inventdelivernow { get; set; }

    public string Inventdimid { get; set; } = null!;

    public string Inventrefid { get; set; } = null!;

    public string Inventreftransid { get; set; } = null!;

    public int Inventreftype { get; set; }

    public string Inventtransid { get; set; } = null!;

    public string Inventtransidreturn { get; set; } = null!;

    public string InvoicegtdidRu { get; set; } = null!;

    public string Itembomid { get; set; } = null!;

    public string Itemid { get; set; } = null!;

    public int Itemreplaced { get; set; }

    public string Itemrouteid { get; set; } = null!;

    public int Itemtagging { get; set; }

    public long Ledgerdimension { get; set; }

    public decimal Lineamount { get; set; }

    public int Linedeliverytype { get; set; }

    public decimal Linedisc { get; set; }

    public string Lineheader { get; set; } = null!;

    public decimal Linenum { get; set; }

    public decimal Linepercent { get; set; }

    public long Manualentrychangepolicy { get; set; }

    public long Matchingagreementline { get; set; }

    public long Mcrorderline2pricehistoryref { get; set; }

    public decimal Multilndisc { get; set; }

    public decimal Multilnpercent { get; set; }

    public string Name { get; set; } = null!;

    public decimal Overdeliverypct { get; set; }

    public string Packingunit { get; set; } = null!;

    public decimal Packingunitqty { get; set; }

    public int Pallettagging { get; set; }

    public int Pdsbatchattribautores { get; set; }

    public decimal Pdscwexpectedretqty { get; set; }

    public decimal Pdscwinventdelivernow { get; set; }

    public decimal Pdscwqty { get; set; }

    public decimal Pdscwremaininventfinancial { get; set; }

    public decimal Pdscwremaininventphysical { get; set; }

    public int Pdsexcludefromrebate { get; set; }

    public string Pdsitemrebategroupid { get; set; } = null!;

    public int Pdssamelot { get; set; }

    public int Pdssamelotoverride { get; set; }

    public string Port { get; set; } = null!;

    public string PostingprofileRu { get; set; } = null!;

    public DateTime PriceagreementdateRu { get; set; }

    public decimal Priceunit { get; set; }

    public string Projcategoryid { get; set; } = null!;

    public string Projid { get; set; } = null!;

    public string Projlinepropertyid { get; set; } = null!;

    public string Projtransid { get; set; } = null!;

    public string PropertynumberMx { get; set; } = null!;

    public string Psacontractlinenum { get; set; } = null!;

    public decimal Psaprojproposalinventqty { get; set; }

    public decimal Psaprojproposalqty { get; set; }

    public string Purchorderformnum { get; set; } = null!;

    public decimal Qtyordered { get; set; }

    public DateTime Receiptdateconfirmed { get; set; }

    public DateTime Receiptdaterequested { get; set; }

    public long RefreturninvoicetransW { get; set; }

    public decimal Remaininventfinancial { get; set; }

    public decimal Remaininventphysical { get; set; }

    public decimal Remainsalesfinancial { get; set; }

    public decimal Remainsalesphysical { get; set; }

    public int Reservation { get; set; }

    public decimal Retailblockqty { get; set; }

    public string Retailvariantid { get; set; } = null!;

    public int Returnallowreservation { get; set; }

    public DateTime Returnarrivaldate { get; set; }

    public DateTime Returncloseddate { get; set; }

    public DateTime Returndeadline { get; set; }

    public string Returndispositioncodeid { get; set; } = null!;

    public int Returnstatus { get; set; }

    public long Salescategory { get; set; }

    public decimal Salesdelivernow { get; set; }

    public string Salesgroup { get; set; } = null!;

    public string Salesid { get; set; } = null!;

    public decimal Salesmarkup { get; set; }

    public decimal Salesprice { get; set; }

    public decimal Salesqty { get; set; }

    public int Salesstatus { get; set; }

    public int Salestype { get; set; }

    public string Salesunit { get; set; } = null!;

    public int Scrap { get; set; }

    public string Serviceorderid { get; set; } = null!;

    public string Shipcarrieraccount { get; set; } = null!;

    public string Shipcarrieraccountcode { get; set; } = null!;

    public int Shipcarrierdlvtype { get; set; }

    public string Shipcarrierid { get; set; } = null!;

    public string Shipcarriername { get; set; } = null!;

    public long Shipcarrierpostaladdress { get; set; }

    public DateTime Shippingdateconfirmed { get; set; }

    public DateTime Shippingdaterequested { get; set; }

    public long Sourcedocumentline { get; set; }

    public decimal StatisticvalueLt { get; set; }

    public string Statprocid { get; set; } = null!;

    public int Stattriangulardeal { get; set; }

    public int Stockedproduct { get; set; }

    public long Systementrychangepolicy { get; set; }

    public int Systementrysource { get; set; }

    public int Taxautogenerated { get; set; }

    public string Taxgroup { get; set; } = null!;

    public string Taxitemgroup { get; set; } = null!;

    public string Taxwithholdgroup { get; set; } = null!;

    public long TaxwithholditemgroupheadingTh { get; set; }

    public string Transactioncode { get; set; } = null!;

    public string Transport { get; set; } = null!;

    public decimal Underdeliverypct { get; set; }

    public long Intrastatcommodity { get; set; }

    public string Origcountryregionid { get; set; } = null!;

    public string Origstateid { get; set; } = null!;

    public string Sourcingcompanyid { get; set; } = null!;

    public string Sourcinginventsiteid { get; set; } = null!;

    public string Sourcinginventlocationid { get; set; } = null!;

    public string Sourcingvendaccount { get; set; } = null!;

    public int Sourcingorigin { get; set; }

    public string OrderlinereferenceNo { get; set; } = null!;

    public int Linecreationsequencenumber { get; set; }

    public string SatproductcodeMx { get; set; } = null!;

    public string SatunitcodeMx { get; set; } = null!;

    public long Accountingdistributiontemplate { get; set; }

    public int ConsignmentMx { get; set; }

    public int SamplesMx { get; set; }

    public string SatcustomunitofmeasureMx { get; set; } = null!;

    public string SattarifffractionMx { get; set; } = null!;

    public decimal Mcrmarginpercent { get; set; }

    public int Domiterations { get; set; }

    public int Domprocessed { get; set; }

    public DateTime Domprocesseddatetime { get; set; }

    public int Domprocesseddatetimetzid { get; set; }

    public int Domrecversion { get; set; }

    public int Domignore { get; set; }

    public int Domexceptiontype { get; set; }

    public string Dataareaid { get; set; } = null!;

    public long Partition { get; set; }

    public long Recid { get; set; }

    public int Recversion { get; set; }

    public DateTime Modifieddatetime { get; set; }

    public string Modifiedby { get; set; } = null!;

    public DateTime Createddatetime { get; set; }

    public string Createdby { get; set; } = null!;

    public decimal SatcustomsqtyMx { get; set; }

    public int Salessalesordercreationmethod { get; set; }

    public string Revrecrevenuescheduleid { get; set; } = null!;

    public int Revrecoccurrences { get; set; }

    public int Revrecbundle { get; set; }

    public int Revrecisbundlecomponent { get; set; }

    public decimal Revrecbundlenetamount { get; set; }

    public string Revrecbundleparent { get; set; } = null!;

    public decimal Revrecbundleqty { get; set; }

    public decimal Revrecbundleratio { get; set; }

    public DateTime Revreccontractenddate { get; set; }

    public DateTime Revreccontractstartdate { get; set; }

    public decimal Revrecbundlelinedisc { get; set; }

    public decimal Revrecbundlelinepercent { get; set; }

    public decimal Revrecbundleqtyordered { get; set; }

    public decimal Revrecbundleremaininventphysical { get; set; }

    public decimal Revrecbundleremainsalesphysical { get; set; }

    public decimal Revrecbundlesalesprice { get; set; }

    public int Revrecbundlesalesstatus { get; set; }

    public string Revrecbundlemainparent { get; set; } = null!;

    public long Projfundingsource { get; set; }

    public decimal Planningpriority { get; set; }

    public int GoodsforfreeIt { get; set; }

    public string CreatedbyparmidIt { get; set; } = null!;

    public int ServicelinetypeIt { get; set; }

    public long PrepaymentrecidIt { get; set; }

    public long EximportsIn { get; set; }

    public long EximproductgroupIn { get; set; }

    public int Kittingskipupdatehelper { get; set; }

    public string Tamrebatetransid { get; set; } = null!;

    public int Overridesalestax { get; set; }

    public int Tamrebateexcluderebatemanagement { get; set; }

    public string Inventoryservicereservationid { get; set; } = null!;

    public int Isfreeitemline { get; set; }

    public long Gupfreeitemlinerecid { get; set; }

    public int Subbillrevenuesplit { get; set; }

    public int Subbillrevenuesplitallocationmethod { get; set; }

    public long Subbillrevenuesplitparentlinerecid { get; set; }

    public decimal Subbillrevenuesplitparentamount { get; set; }

    public int Subbillisrevenuesplitchild { get; set; }
}

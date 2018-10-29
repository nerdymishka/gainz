
$skuPartMap = @{
    STREAM = "Stream"
    DEVELOPERPACK = "o365 E3 Developer License"
    ENTERPRISEPACK = "o365 E3 License"
    ENTERPRISEPREMIUM = "o365 E5 License"
    EMS = "Microsoft Enterprise Mobility E3"
    EMSPREMIUM = "Microsoft Enterprise Mobility E5"
    POWERAPPS_INDIVIDUAL_USER = "Power Apps Single License"
    POWER_BI_PRO = "Power BI Pro License"
    POWER_BI_STANDARD = "Power BI"
    FLOW_FREE = "Microsoft Flow Free License"
    STREAM_O365_E3 = "Microsoft Stream E3 License"
    FORMS_PLAN_E3 = "Microsoft Forms E3 License"
}

function Get-AzureAdSkus() {

    $adSkus = Get-AzureADSubscribedSku
    

    $set = @();

    # TODO: handle service plans

    foreach($adSku in $adSkus)
    {
        $displayName = $null
        if($skuPartMap.ContainsKey($adSku.SkuPartNumber)) {
            $displayName = $skuPartMap[$adSku.SkuPartNumber]
        }

        $license = [PSCustomObject]@{
            displayName = $displayName
            name = $adSku.SkuPartNumber
            azureAdObjectId = $adSku.ObjectId 
            id = $adSku.Id 
            prepaidUnits = $adSku.prepaidUnits.Enabled 
            consumedUnits = $adSku.consumedUnits 
        }

        $set += $license
    }
    
    return $set;
}
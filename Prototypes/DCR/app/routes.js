//
// For guidance on how to create routes see:
// https://prototype-kit.service.gov.uk/docs/create-routes
//

const govukPrototypeKit = require('govuk-prototype-kit')
const router = govukPrototypeKit.requests.setupRouter()

// Add your routes here

// Section 2 - Company Details pages

router.post('/tradingAddress-answer', function(request, response) {

	var tradingAddress = request.session.data['tradingAddress']
	if (tradingAddress == "yes"){
		response.redirect("/section-02-03-company-details-trading-address")
	} else {
		response.redirect("/section-02-04-company-details-failed-companies")
	}
})

router.post('/failedCompanies-answer', function(request, response) {

	var failedCompanies = request.session.data['failedCompanies']
	if (failedCompanies == "yes"){
		response.redirect("/section-02-05-company-details-linked-companies")
	} else {
		response.redirect("/section-02-06-company-details-bank-details")
	}
})

// Section 3 - Director Details
// Director Details Overview

router.post('/addDirector-answer', function(request, response) {

	var addDirector = request.session.data['addDirector']
	if (addDirector == "individual"){
		response.redirect("/section-03a-01-individual-director-details")
	} else if (addDirector == "corporate") {
		response.redirect("/section-03b-01-corporate-director-details")
	} else {response.redirect("/section-04-01-company-insolvency-creditors")
	}
})

// Individual Director

router.post('/knownByOtherNames-answer', function(request, response) {

	var knownByOtherNames = request.session.data['knownByOtherNames']
	if (knownByOtherNames == "yes"){
		response.redirect("/section-03a-02-individual-director-alias")
	} else {
		response.redirect("/section-03a-03-individual-director-address")
	}
})

router.post('/directorDeceased-answer', function(request, response) {

	var directorDeceased = request.session.data['directorDeceased']
	if (directorDeceased == "yes"){
		response.redirect("/section-03a-05-individual-director-deceased-sole")
	} else {
		response.redirect("/section-03a-06-individual-director-recorded-companies-house")
	}
})

router.post('/soleDirector-answer', function(request, response) {

	var soleDirector = request.session.data['soleDirector']
	if (soleDirector == "yes"){
		response.redirect("/submit-this-report")
	} else {
		response.redirect("/section-03-01-director-details")
	}
})

router.post('/recordedAtCompaniesHouse-answer', function(request, response) {

	var recordedAtCompaniesHouse = request.session.data['recordedAtCompaniesHouse']
	if (recordedAtCompaniesHouse == "yes"){
		response.redirect("/section-03a-07-individual-director-companies-house-dates")
	} else {
		response.redirect("/section-03a-08-individual-director-acted-director")
	}
})

router.post('/evidenceActedAsDirector-answer', function(request, response) {

	var evidenceActedAsDirector = request.session.data['evidenceActedAsDirector']
	if (evidenceActedAsDirector == "yes"){
		response.redirect("/section-03a-09-individual-director-evidence-acted-director")
	} else {
		response.redirect("/section-03a-10-individual-director-third-failure")
	}
})

router.post('/actedWhileRestricted-answer', function(request, response) {

	var actedWhileRestricted = request.session.data['actedWhileRestricted']
	if (actedWhileRestricted == "yes"){
		response.redirect("/section-03a-12-individual-director-evidence-restricted")
	} else {
		response.redirect("/section-03a-13-individual-director-further-information")
	}
})

// Corporate Director

router.post('/corpInsolvencyOrDissolved-answer', function(request, response) {

	var corpInsolvencyOrDissolved = request.session.data['corpInsolvencyOrDissolved']
	if (corpInsolvencyOrDissolved == "yes"){
		response.redirect("/section-03b-04-corporate-director-insolvency-details")
	} else {
		response.redirect("/section-03b-05-corporate-director-recorded-companies-house")
	}
})

router.post('/corpRecordedAtCompaniesHouse-answer', function(request, response) {

	var corpRecordedAtCompaniesHouse = request.session.data['corpRecordedAtCompaniesHouse']
	if (corpRecordedAtCompaniesHouse == "yes"){
		response.redirect("/section-03b-06-corporate-director-companies-house-dates")
	} else {
		response.redirect("/section-03b-07-corporate-director-acted-director")
	}
})

router.post('/corpEvidenceActedAsDirectorCorp-answer', function(request, response) {

	var corpEvidenceActedAsDirectorCorp = request.session.data['corpEvidenceActedAsDirectorCorp']
	if (corpEvidenceActedAsDirectorCorp == "yes"){
		response.redirect("/section-03b-08-corporate-director-evidence-acted-director")
	} else {
		response.redirect("/section-03b-09-corporate-director-third-failure")
	}
})

// Section 4 - Company Insolvency pages

router.post('/majorityCreditor-answer', function(request, response) {

	var majorityCreditor = request.session.data['majorityCreditor']
	if (majorityCreditor == "yes"){
		response.redirect("/section-04-02-company-insolvency-majority-creditor")
	} else {
		response.redirect("/section-04-03-company-insolvency-creditor-payments")
	}
})

router.post('/unsecuredDividend-answer', function(request, response) {

	var unsecuredDividend = request.session.data['unsecuredDividend']
	if (unsecuredDividend == "yes"){
		response.redirect("/section-04-04-company-insolvency-dividends")
	} else {
		response.redirect("/section-04-05-company-insolvency-action-to-recover")
	}
})

router.post('/plannedAction-answer', function(request, response) {

	var plannedAction = request.session.data['plannedAction']
	if (plannedAction == "yes"){
		response.redirect("/section-04-06-company-insolvency-fund-recovery")
	} else {
		response.redirect("/section-04-07-company-insolvency-type-of-insolvency")
	}
})

router.post('/insolvencyType-answer', function(request, response) {

	var insolvencyType = request.session.data['insolvencyType']
	if (insolvencyType == "liquidation"){
		response.redirect("/section-04-08-company-insolvency-live-business")
	} else {
		response.redirect("/section-04-11-company-insolvency-explanation-values")
	}
})

router.post('/liveBusiness-answer', function(request, response) {

	var liveBusiness = request.session.data['liveBusiness']
	if (liveBusiness == "yes"){
		response.redirect("/section-04-09-company-insolvency-s216-exceptions")
	} else {
		response.redirect("/section-04-11-company-insolvency-explanation-values")
	}
})

router.post('/s216Exceptions-answer', function(request, response) {

	var s216Exceptions = request.session.data['s216Exceptions']
	if (s216Exceptions == "yes"){
		response.redirect("/section-04-11-company-insolvency-explanation-values")
	} else {
		response.redirect("/section-04-10-company-insolvency-names-of-live-business")
	}
})

router.post('/companyDeficiency-answer', function(request, response) {

	var companyDeficiency = request.session.data['companyDeficiency']
	if (companyDeficiency == "yes"){
		response.redirect("/section-05-01-books-and-records")
	} else {
		response.redirect("/section-14-01-bounce-back-loans")
	}
})

// Section 5 - Books and Records

router.post('/booksRecords-answer', function(request, response) {

	var booksRecords = request.session.data['booksRecords']
	if (booksRecords == "no"){
		response.redirect("/section-05-02-books-and-records-change-explained")
	} else {
		response.redirect("/section-06-01-unreasonable-benefits")
	}
})

router.post('/changeExplained-answer', function(request, response) {

	var changeExplained = request.session.data['changeExplained']
	if (changeExplained == "yes"){
		response.redirect("/section-06-01-unreasonable-benefits")
	} else {
		response.redirect("/section-05-03-books-and-records-deficiencies-harder")
	}
})

router.post('/deficienciesHarder-answer', function(request, response) {

	var deficienciesHarder = request.session.data['deficienciesHarder']
	if (deficienciesHarder == "yes"){
		response.redirect("/section-05-04-books-and-records-deficiency-value")
	} else {
		response.redirect("/section-06-01-unreasonable-benefits")
	}
})

router.post('/booksAndRecordsDeficiencyValue-answer', function(request, response) {

	var deficiencyValue = request.session.data['deficiencyValue']
	if (deficiencyValue == "yes"){
		response.redirect("/section-05-05-books-and-records-further-information")
	} else {
		response.redirect("/section-06-01-unreasonable-benefits")
	}
})

// Section 6 - Unreasonable Benefits

router.post('/unreasonableBenefitsEvidence-answer', function(request, response) {

	var unreasonableBenefitsEvidence = request.session.data['unreasonableBenefitsEvidence']
	if (unreasonableBenefitsEvidence == "yes"){
		response.redirect("/section-06-02-unreasonable-benefits-value")
	} else {
		response.redirect("/section-07-01-insolvent-trading")
	}
})

router.post('/benefitsTimingValue-answer', function(request, response) {

	var benefitsTimingValue = request.session.data['benefitsTimingValue']
	if (benefitsTimingValue == "noneOfTheAbove"){
		response.redirect("/section-07-01-insolvent-trading")
	} else {
		response.redirect("/section-06-03-unreasonable-benefits-further-information")
	}
})

// Section 7 - Insolvent Trading

router.post('/newDebtsWhileInsolvent-answer', function(request, response) {

	var newDebtsWhileInsolvent = request.session.data['newDebtsWhileInsolvent']
	if (newDebtsWhileInsolvent == "yes"){
		response.redirect("/section-07-02-insolvent-trading-sought-advice")
	} else {
		response.redirect("/section-08-01-detrimental-transactions")
	}
})

router.post('/soughtAdvice-answer', function(request, response) {

	var soughtAdvice = request.session.data['soughtAdvice']
	if (soughtAdvice == "yes"){
		response.redirect("/section-07-03-insolvent-trading-acted-on-advice")
	} else {
		response.redirect("/section-07-04-insolvent-trading-reasonable-belief")
	}
})


router.post('/actedOnAdvice-answer', function(request, response) {

	var actedOnAdvice = request.session.data['actedOnAdvice']
	if (actedOnAdvice == "no"){
		response.redirect("/section-07-04-insolvent-trading-reasonable-belief")
	} else {
		response.redirect("/section-08-01-detrimental-transactions")
	}
})

router.post('/reasonableBeliefRecovery-answer', function(request, response) {

	var reasonableBeliefRecovery = request.session.data['reasonableBeliefRecovery']
	if (reasonableBeliefRecovery == "no"){
		response.redirect("/section-07-05-insolvent-trading-introduced-funds")
	} else {
		response.redirect("/section-08-01-detrimental-transactions")
	}
})

router.post('/introducedFundsOrReducedBenefits-answer', function(request, response) {

	var introducedFundsOrReducedBenefits = request.session.data['introducedFundsOrReducedBenefits']
	if (introducedFundsOrReducedBenefits == "no"){
		response.redirect("/section-07-06-insolvent-trading-further-information")
	} else {
		response.redirect("/section-08-01-detrimental-transactions")
	}
})

// Section 8 - Detrimental Transactions

router.post('/unfairAdvantageTransactions-answer', function(request, response) {

	var unfairAdvantageTransactions = request.session.data['unfairAdvantageTransactions']
	if (unfairAdvantageTransactions == "yes"){
		response.redirect("/section-08-02-detrimental-transactions-value")
	} else {
		response.redirect("/section-09-01-tax-affairs")
	}
})

router.post('/whenValueTransactions-answer', function(request, response) {

	var whenValueTransactions = request.session.data['whenValueTransactions']
	if (whenValueTransactions == "noneOfTheAbove"){
		response.redirect("/section-09-01-tax-affairs")
	} else {
		response.redirect("/section-08-03-detrimental-transactions-further-information")
	}
})

// Section 9 - Tax Affairs

router.post('/failedTaxAffairs-answer', function(request, response) {

	var failedTaxAffairs = request.session.data['failedTaxAffairs']
	if (failedTaxAffairs == "yes"){
		response.redirect("/section-09-02-tax-affairs-fell-behind")
	} else {
		response.redirect("/section-10-01-asset-disposals")
	}
})

router.post('/fellBehindHMRC6m-answer', function(request, response) {

	var fellBehindHMRC6m = request.session.data['fellBehindHMRC6m']
	if (fellBehindHMRC6m == "yes"){
		response.redirect("/section-09-03-tax-affairs-time-to-pay-arrangement")
	} else {
		response.redirect("/section-10-01-asset-disposals")
	}
})

router.post('/timeToPayArrangement-answer', function(request, response) {

	var timeToPayArrangement = request.session.data['timeToPayArrangement']
	if (timeToPayArrangement == "yes"){
		response.redirect("/section-09-04-tax-affairs-time-to-pay-compliance")
	} else {
		response.redirect("/section-09-05-tax-affairs-hmrc-liabilities-percentage")
	}
})

router.post('/compliedTimeToPay-answer', function(request, response) {

	var compliedTimeToPay = request.session.data['compliedTimeToPay']
	if (compliedTimeToPay == "no"){
		response.redirect("/section-09-05-tax-affairs-hmrc-liabilities-percentage")
	} else {
		response.redirect("/section-10-01-asset-disposals")
	}
})

router.post('/hmrcOverHalfLiabilities-answer', function(request, response) {

	var hmrcOverHalfLiabilities = request.session.data['hmrcOverHalfLiabilities']
	if (hmrcOverHalfLiabilities == "yes"){
		response.redirect("/section-09-06-tax-affairs-further-information")
	} else {
		response.redirect("/section-10-01-asset-disposals")
	}
})

router.post('/persistentLateFiling-answer', function(request, response) {

	var persistentLateFiling = request.session.data['persistentLateFiling']
	if (persistentLateFiling == "yes"){
		response.redirect("/section-09-07-tax-affairs-fees-or-penalties")
	} else {
		response.redirect("/section-10-01-asset-disposals")
	}
})

// Section 10 - Asset Disposals

router.post('/assetsSoldBelowValue-answer', function(request, response) {

	var assetsSoldBelowValue = request.session.data['assetsSoldBelowValue']
	if (assetsSoldBelowValue == "yes"){
		response.redirect("/section-10-02-asset-disposals-value")
	} else {
		response.redirect("/section-11-01-successor-trading")
	}
})

router.post('/whenValueAssetDisposals-answer', function(request, response) {

	var whenValueAssetDisposals = request.session.data['whenValueAssetDisposals']
	if (whenValueAssetDisposals == "noneOfTheAbove"){
		response.redirect("/section-11-01-successor-trading")
	} else {
		response.redirect("/section-10-03-asset-disposals-further-information")
	}
})


// Section 11 - Successor Trading

router.post('/similarToPreviousFailures-answer', function(request, response) {

	var similarToPreviousFailures = request.session.data['similarToPreviousFailures']
	if (similarToPreviousFailures == "yes"){
		response.redirect("/section-11-02-successor-trading-director-involvement")
	} else {
		response.redirect("/section-12-01-customer-treatment")
	}
})

router.post('/directorsInvolvedBefore-answer', function(request, response) {

	var directorsInvolvedBefore = request.session.data['directorsInvolvedBefore']
	if (directorsInvolvedBefore == "yes"){
		response.redirect("/section-11-03-successor-trading-new-finance")
	} else {
		response.redirect("/section-12-01-customer-treatment")
	}
})

router.post('/newFinanceIntroduced-answer', function(request, response) {

	var newFinanceIntroduced = request.session.data['newFinanceIntroduced']
	if (newFinanceIntroduced == "no"){
		response.redirect("/section-11-04-successor-trading-changes-to-improve")
	} else {
		response.redirect("/section-12-01-customer-treatment")
	}
})

router.post('/changesToImproveViability-answer', function(request, response) {

	var changesToImproveViability = request.session.data['changesToImproveViability']
	if (changesToImproveViability == "no"){
		response.redirect("/section-11-05-successor-trading-belief-viability")
	} else {
		response.redirect("/section-12-01-customer-treatment")
	}
})

router.post('/believedRemainViable-answer', function(request, response) {

	var believedRemainViable = request.session.data['believedRemainViable']
	if (believedRemainViable == "no"){
		response.redirect("/section-11-06-successor-trading-profit")
	} else {
		response.redirect("/section-12-01-customer-treatment")
	}
})

router.post('/everMadeProfit-answer', function(request, response) {

	var everMadeProfit = request.session.data['everMadeProfit']
	if (everMadeProfit == "no"){
		response.redirect("/section-11-07-successor-trading-further-information")
	} else {
		response.redirect("/section-12-01-customer-treatment")
	}
})

// Section 12 - Customer Treatment

router.post('/failedGoodsOrSafeguarding-answer', function(request, response) {

	var failedGoodsOrSafeguarding = request.session.data['failedGoodsOrSafeguarding']
	if (failedGoodsOrSafeguarding == "yes"){
		response.redirect("/section-12-02-customer-treatment-further-information")
	} else {
		response.redirect("/section-13-01-financial-support-schemes")
	}
})

// Section 13 - Financial Support Schemes

router.post('/schemeMisuse-answer', function(request, response) {

	var schemeMisuse = request.session.data['schemeMisuse']
	if (schemeMisuse == "yes"){
		response.redirect("/section-13-02-financial-support-schemes-value")
	} else {
		response.redirect("/section-14-01-bounce-back-loans")
	}
})

router.post('/fundingOver25k-answer', function(request, response) {

	var fundingOver25k = request.session.data['fundingOver25k']
	if (fundingOver25k == "yes"){
		response.redirect("/section-13-03-financial-support-schemes-further-information")
	} else {
		response.redirect("/section-14-01-bounce-back-loans")
	}
})

// Section 14 - Bounce Back Loans

router.post('/bounceBack-answer', function(request, response) {

	var bounceBack = request.session.data['bounceBack']
	if (bounceBack == "yes"){
		response.redirect("/section-14-02-bounce-back-loans-details")
	} else {
		response.redirect("/section-15-01-other-investigations")
	}
})

// Section 15 - Other Investigations

router.post('/mattersForRegulatorsOrPolice-answer', function(request, response) {

	var mattersForRegulatorsOrPolice = request.session.data['mattersForRegulatorsOrPolice']
	if (mattersForRegulatorsOrPolice == "yes"){
		response.redirect("/section-15-02-other-investigations-status")
	} else {
		response.redirect("/report-overview")
	}
})

router.post('/existingInvestigations-answer', function(request, response) {

	var existingInvestigations = request.session.data['existingInvestigations']
	if (existingInvestigations == "yes"){
		response.redirect("/section-15-03-other-investigations-existing-investigations")
	} else {
		response.redirect("/report-overview")
	}
})

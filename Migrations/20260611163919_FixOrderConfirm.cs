using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareProjct.web.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderConfirm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "Caretaker",
                newName: "VerificationStatus");

            migrationBuilder.RenameColumn(
                name: "CardholderName",
                table: "Caretaker",
                newName: "RejectionReason");

            migrationBuilder.RenameColumn(
                name: "CardNumber",
                table: "Caretaker",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "CVV",
                table: "Caretaker",
                newName: "PoliceClearancePath");

            migrationBuilder.AddColumn<bool>(
                name: "AcceptedBookingTerms",
                table: "OrderConfirm",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BookingStatus",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsRenewal",
                table: "OrderConfirm",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDays",
                table: "OrderConfirm",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NurseRejectionReason",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PatientAge",
                table: "OrderConfirm",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PatientCondition",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientName",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientNotes",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PreviousBookingId",
                table: "OrderConfirm",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RenewalCount",
                table: "OrderConfirm",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RenewalReminderSent",
                table: "OrderConfirm",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ServiceAddress",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShiftType",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TermsAcceptedOn",
                table: "OrderConfirm",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AadhaarPath",
                table: "Caretaker",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountHolderName",
                table: "Caretaker",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "AgreedToBackgroundCheck",
                table: "Caretaker",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AgreedToNDA",
                table: "Caretaker",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AgreedToNurseAgreement",
                table: "Caretaker",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AgreementSignedOn",
                table: "Caretaker",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "Caretaker",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Caretaker",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CurrentRenewalCount",
                table: "Caretaker",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IFSCCode",
                table: "Caretaker",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LicenseDocumentPath",
                table: "Caretaker",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Caretaker",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxRenewalsAllowed",
                table: "Caretaker",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Caretaker",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedOn",
                table: "Caretaker",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedBookingTerms",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "BookingStatus",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "IsRenewal",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "NumberOfDays",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "NurseRejectionReason",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "PatientAge",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "PatientCondition",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "PatientName",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "PatientNotes",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "PreviousBookingId",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "RenewalCount",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "RenewalReminderSent",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "ServiceAddress",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "ShiftType",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "TermsAcceptedOn",
                table: "OrderConfirm");

            migrationBuilder.DropColumn(
                name: "AadhaarPath",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "AccountHolderName",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "AgreedToBackgroundCheck",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "AgreedToNDA",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "AgreedToNurseAgreement",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "AgreementSignedOn",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "CurrentRenewalCount",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "IFSCCode",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "LicenseDocumentPath",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "MaxRenewalsAllowed",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Caretaker");

            migrationBuilder.DropColumn(
                name: "VerifiedOn",
                table: "Caretaker");

            migrationBuilder.RenameColumn(
                name: "VerificationStatus",
                table: "Caretaker",
                newName: "ExpirationDate");

            migrationBuilder.RenameColumn(
                name: "RejectionReason",
                table: "Caretaker",
                newName: "CardholderName");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Caretaker",
                newName: "CardNumber");

            migrationBuilder.RenameColumn(
                name: "PoliceClearancePath",
                table: "Caretaker",
                newName: "CVV");
        }
    }
}

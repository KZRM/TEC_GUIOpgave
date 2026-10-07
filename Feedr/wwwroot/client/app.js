$(function () {
    const apiUrl = window.fileClientConfig.apiBaseUrl.replace(/\/+$/, "") + "/api/files";
    const maxFileSize = 10 * 1024 * 1024;
    const rowTemplate = document.getElementById("file-row");
    let files = [];
    let loading = false;
    let uploading = false;

    function showStatus(message, isError = false) {
        $("#status").text(message).prop("hidden", !message)
            .toggleClass("error", isError).attr("role", isError ? "alert" : "status");
    }

    function formatSize(bytes) {
        return bytes < 1024 ? `${bytes} B` : `${(bytes / 1024).toLocaleString("da-DK", { maximumFractionDigits: 1 })} KB`;
    }

    function renderFiles() {
        const rows = document.createDocumentFragment();
        for (const file of files) {
            const row = rowTemplate.content.cloneNode(true);
            // 2.3: .text() viser filnavne som tekst og forhindrer HTML/XSS.
            $(row).find(".file-name").text(file.name);
            $(row).find(".file-size").text(formatSize(file.size));
            $(row).find(".file-date").text(new Date(file.uploadedAtUtc).toLocaleString("da-DK"));
            $(row).find(".download-button").attr("data-id", file.id).attr("aria-label", `Download ${file.name}`);
            rows.append(row);
        }
        // 2.2.4 / 2.3: Kun tabelindholdet ændres, samlet og uden sidegenindlæsning.
        $("#file-list").empty().append(rows);
        $("#file-count").text(`${files.length} filer`);
        $("#empty-message").prop("hidden", files.length !== 0);
        $("#file-table").prop("hidden", files.length === 0);
    }

    async function loadFiles() {
        if (loading) return false;
        loading = true;
        $("#refresh-button, #upload-button").prop("disabled", true);
        $("#file-list").attr("aria-busy", "true");
        try {
            // 2.2.3 / 2.2.6: jQuery AJAX henter listen asynkront som JS-objekter.
            files = await $.ajax({ url: apiUrl, method: "GET", dataType: "json", cache: false });
            renderFiles();
            return true;
        } catch {
            $("#file-count").text("Oversigten kunne ikke opdateres.");
            showStatus("Kunne ikke hente filerne. Kontroller forbindelsen og prøv igen.", true);
            return false;
        } finally {
            loading = false;
            $("#refresh-button").prop("disabled", false);
            $("#upload-button").prop("disabled", uploading);
            $("#file-list").attr("aria-busy", "false");
        }
    }

    async function uploadFile(event) {
        event.preventDefault();
        if (uploading || loading) return;
        const file = $("#file")[0].files[0];

        // 2.3: Klientvalidering giver hurtig feedback før API-kaldet.
        if (!file || !/\.(txt|pdf|docx|csv)$/i.test(file.name)) {
            showStatus("Vælg en .txt-, .pdf-, .docx- eller .csv-fil.", true);
            return;
        }
        if (file.size === 0 || file.size > maxFileSize) {
            showStatus("Filen skal indeholde data og må højst fylde 10 MB.", true);
            return;
        }

        const data = new FormData();
        data.append("file", file);
        uploading = true;
        $("#upload-button, #file").prop("disabled", true);
        showStatus("Uploader fil...");
        try {
            // 2.2.4: FormData sender filens bytes som multipart/form-data.
            const saved = await $.ajax({
                url: apiUrl, method: "POST", data,
                processData: false, contentType: false
            });
            $("#upload-form")[0].reset();
            if (await loadFiles()) {
                showStatus(`${saved.name} er uploadet.`);
            } else {
                showStatus("Filen er uploadet, men oversigten kunne ikke hentes. Tryk Opdater oversigt.", true);
            }
        } catch (error) {
            showStatus(error.responseJSON?.message ?? "Filen kunne ikke uploades. Prøv igen.", true);
        } finally {
            uploading = false;
            $("#upload-button").prop("disabled", loading);
            $("#file").prop("disabled", false);
        }
    }

    async function downloadFile(file, button) {
        button.prop("disabled", true);
        showStatus(`Henter ${file.name}...`);
        try {
            // 2.2.6: Blob bevarer indholdet i både tekstfiler, PDF og DOCX.
            const blob = await $.ajax({
                url: `${apiUrl}/${encodeURIComponent(file.id)}/download`,
                method: "GET", xhrFields: { responseType: "blob" }
            });
            const url = URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.href = url;
            link.download = file.name;
            document.body.append(link);
            link.click();
            link.remove();
            setTimeout(() => URL.revokeObjectURL(url), 1000);
            showStatus(`${file.name} er hentet.`);
        } catch (error) {
            showStatus(error.status === 404 ? "Filen er slettet fra serveren. Opdater oversigten."
                : "Filen kunne ikke hentes. Prøv igen.", true);
        } finally {
            button.prop("disabled", false);
        }
    }

    // 2.2.5: jQuery-events håndterer formularen og knapperne, også nye tabelrækker.
    $("#upload-form").on("submit", uploadFile);
    $("#refresh-button").on("click", function () {
        showStatus("");
        loadFiles();
    });
    $("#file-list").on("click", ".download-button", function () {
        const button = $(this);
        const file = files.find(file => file.id === button.attr("data-id"));
        if (file) downloadFile(file, button);
    });
    loadFiles();
});

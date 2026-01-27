console.log("rybbit-events.js loaded and running.");

let formCompletedEventSent = false;
let scrollEventSent = false;
let formSubmittedEventSent = new Set();

function sendFormCompletedEvent() {
    if (formCompletedEventSent)
        return true; 

    var formPreSave = document.getElementById("analytics-form-pre-save");

    if (formPreSave && window.rybbit) {
        var eventData = {};

        if (formPreSave.dataset.incomeSource) {
            eventData.incomeSource = formPreSave.dataset.incomeSource;
            eventData.incomeGross = formPreSave.dataset.incomeGross;
            eventData.incomeFrequency = formPreSave.dataset.incomeFrequency;

            const gross = parseFloat(formPreSave.dataset.incomeGross);
            if (!isNaN(gross)) {
                eventData.isHighIncomeGross = gross >= 1000;
            }
        }

        if (Object.keys(eventData).length > 0) {
            window.rybbit.event("Form: PreSave", eventData);
            formCompletedEventSent = true;
            return true;
        }
    }

    return false;
}

function sendFormSubmittedEvent(form) {
    let key = new URL(form.action, window.location.origin).pathname;

    if (formSubmittedEventSent.has(key))
        return; 

    if (window.rybbit) {
        window.rybbit.event("Form: Submission", {
            formId: key,
            env: "Development"
        });
        formSubmittedEventSent.add(key);
    }
}

function sendScrollEvent() {
    if (scrollEventSent) {
        return;
    }
    const scrolledToBottom = (window.innerHeight + window.scrollY +1) >= document.body.offsetHeight;
    if (scrolledToBottom)
    {
        if (window.rybbit) {
            window.rybbit.event("Scroll: End", { page: window.location.pathname });
            scrollEventSent = true;
        }
    }
}

function sendFormCompletedEventWhenReady() {
    if (sendFormCompletedEvent()) {
        return;
    }

    var interval = setInterval(function () {
        if (sendFormCompletedEvent()) {
            clearInterval(interval);
        }
    }, 100);
}

function sendScrollEventWhenReady() {
    if (sendScrollEvent()) {
        return;
    }
    var interval = setInterval(function () {
        if (sendScrollEvent()) {
            clearInterval(interval);
        }
    }, 100);
}

function setupFormSubmissionEvents() {
    document.querySelectorAll("form").forEach(form => {
        form.addEventListener("submit", function () {
            sendFormSubmittedEvent(form);
        });
    });
}

function setupScrollEvents() {
    window.addEventListener("scroll", () => {
        sendScrollEventWhenReady();
    });
}

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", function () {
        sendFormCompletedEventWhenReady();
        setupFormSubmissionEvents();
        setupScrollEvents();
    });
}
else {
    sendFormCompletedEventWhenReady();
    setupFormSubmissionEvents();
    setupScrollEvents();
}

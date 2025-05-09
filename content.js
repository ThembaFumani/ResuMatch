console.log("Resumatch Content script loaded");

const urlHandlers = {
    "/https:\\/\\/(www\\.)?linkedin\\.com\\/jobs\\/view\\/\\d+/": "LinkedIn",
    "/https:\\/\\/(www\\.)?indeed\\.com\\/viewjob\\?jk=\\w+/": "Indeed",
    "/https:\\/\\/(www\\.)?glassdoor\\.com\\/Job\\/\\w+-\\w+-\\w+\\.htm/": "Glassdoor",
    "/https:\\/\\/(www\\.)?careerbuilder\\.com\\/jobseeker\\/jobdetails\\?id=\\w+/": "CareerBuilder",
    "default": "Generic"
};

function extractJobDescription(source) {
    let description = "";
    try {
        let descriptionElement =
            document.querySelector(".job-description") ||
            document.querySelector('[data-automation="job-description"]') ||
            document.getElementById("jobDescriptionText") ||
            document.querySelector(".show-more-less-html__markup") ||
            document.querySelector(".description") ||
            document.querySelector('div[data-automation="job-details-description-content"]') ||
            document.querySelector('[role="tabpanel"]');

        if (descriptionElement) {
            description = descriptionElement.textContent.trim();
        }
    } catch (error) {
        console.error(`Error extracting job description from ${source}:`, error);
    }
    return description;
}

// Listen for messages from the popup
chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.action === "getJobDescription") {
        const currentURL = window.location.href;
        let jobDescription = null;
        for (const pattern in urlHandlers) {
            const source = urlHandlers[pattern];
            if (pattern.startsWith('/') && pattern.endsWith('/')) {
                const regexPattern = pattern.slice(1, -1);
                const regex = new RegExp(regexPattern);
                if (regex.test(currentURL)) {
                    jobDescription = extractJobDescription(source);
                    break;
                }
            } else if (currentURL.startsWith(pattern)) {
                jobDescription = extractJobDescription(source);
                break;
            }
        }
        if (!jobDescription) {
            jobDescription = extractJobDescription("default");
        }
        sendResponse({ jobDescription: jobDescription });
        return true; // Indicate that you wish to send a response asynchronously
    }
});

// Optionally, you can keep the automatic job description sending on page load/URL change
function handleURL() {
    const currentURL = window.location.href;
    let jobDescription = null;
    for (const pattern in urlHandlers) {
        const source = urlHandlers[pattern];
        if (pattern.startsWith('/') && pattern.endsWith('/')) {
            const regexPattern = pattern.slice(1, -1);
            const regex = new RegExp(regexPattern);
            if (regex.test(currentURL)) {
                jobDescription = extractJobDescription(source);
                break;
            }
        } else if (currentURL.startsWith(pattern)) {
            jobDescription = extractJobDescription(source);
            break;
        }
    }
    if (!jobDescription) {
        jobDescription = extractJobDescription("default");
    }
    chrome.runtime.sendMessage({ action: "jobDescription", jobDescription: jobDescription });
}

// Execute the URL handling logic when the content script loads
handleURL();

// Listen for URL changes (e.g., due to single-page app navigation)
window.addEventListener('popstate', handleURL);
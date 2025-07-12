
// Listen for messages from the popup
chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.action === "extractJobDescription") {
        const currentURL = window.location.href;
        const linkedinRegex = /https:\/\/(\w+\.)?linkedin\.com\/jobs\/view\/[\w-]+(\?\S*)?/;

        if (linkedinRegex.test(currentURL)) {
            const jobDescription = extractLinkedInDescription();
            sendResponse({ jobDescription: jobDescription });
        } else {
            sendResponse({ jobDescription: null, error: "Not a LinkedIn job posting page." });
        }
        return true; // Indicate that you wish to send a response asynchronously
    }
});

console.log("Resumatch Content script loaded (LinkedIn only)");


// Function to extract job description from LinkedIn
function extractLinkedInDescription() {
    try {
        const descriptionParagraphs = document.querySelectorAll(
            ".jobs-box__html-content > div > p"
        );
        if (descriptionParagraphs) {
            let descriptionText = "";
            descriptionParagraphs.forEach((paragraph) => {
                descriptionText += paragraph.innerText + "\n";
            });
            return descriptionText.trim();
        }
    } catch (error) {
        console.error("Error extracting job description from LinkedIn:", error);
    }
    return null;
}

function handleURL() {
    const currentURL = window.location.href;
    const linkedinRegex = /https:\/\/(\w+\.)?linkedin\.com\/jobs\/view\/\d+/;

    if (linkedinRegex.test(currentURL)) {
        const jobDescription = extractLinkedInDescription();
        chrome.runtime.sendMessage({ action: "jobDescription", jobDescription: jobDescription });
    } else {
        chrome.runtime.sendMessage({ action: "jobDescription", jobDescription: null });
    }
}

// Execute the URL handling logic when the content script loads
handleURL();

// Listen for URL changes (e.g., due to single-page app navigation)
window.addEventListener('popstate', handleURL);
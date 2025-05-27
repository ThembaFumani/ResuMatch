// Listen for messages from the popup
chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.action === "extractJobDescription") {
        const currentURL = window.location.href;
        // The regex below handles both /jobs/view/ (with optional query params) and /jobs/collections/ URLs
        const linkedinRegex = /https:\/\/(\w+\.)?linkedin\.com\/jobs\/(?:view\/[\w-]+(?:\/?\?\S*)?|collections\/recommended\/\?currentJobId=\d+)/;

        if (linkedinRegex.test(currentURL)) {
            const jobDescription = extractLinkedInDescription();
            sendResponse({ jobDescription: jobDescription });
        } else {
            sendResponse({ jobDescription: null, error: "Not a supported LinkedIn job posting page." });
        }
        return true; // Indicate that you wish to send a response asynchronously
    }
});

console.log("Resumatch Content script loaded (LinkedIn only)");


// Function to extract job description from LinkedIn
// Note: You might need to adjust this function if the HTML structure
// for job descriptions differs significantly between the two URL types.
function extractLinkedInDescription() {
    try {
        // This selector generally works for the main description container
        // on both types of LinkedIn job pages.
        // You might need to refine this if you find inconsistencies.
        const descriptionContainer = document.querySelector(".jobs-description__content");
        if (descriptionContainer) {
            // Get the innerText of the div containing the description
            return descriptionContainer.innerText.trim();
        }

        // Fallback for older/different LinkedIn layouts if needed
        const descriptionParagraphs = document.querySelectorAll(
            ".jobs-box__html-content > div > p"
        );
        if (descriptionParagraphs && descriptionParagraphs.length > 0) {
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


// --- Existing handleURL and event listeners ---

function handleURL() {
    const currentURL = window.location.href;
    // Use the same comprehensive regex here
    const comprehensiveLinkedinRegex = /https:\/\/(\w+\.)?linkedin\.com\/jobs\/(?:view\/[\w-]+(?:\/?\?\S*)?|collections\/recommended\/\?currentJobId=\d+)/;


    if (comprehensiveLinkedinRegex.test(currentURL)) {
        const jobDescription = extractLinkedInDescription();
        // Only send a message if a description was actually found
        if (jobDescription) {
            chrome.runtime.sendMessage({ action: "jobDescription", jobDescription: jobDescription });
        } else {
            // If no description found, send null but indicate it's a LinkedIn page
            chrome.runtime.sendMessage({ action: "jobDescription", jobDescription: null, error: "LinkedIn page, but description not found." });
        }
    } else {
        chrome.runtime.sendMessage({ action: "jobDescription", jobDescription: null, error: "Not a supported LinkedIn job page." });
    }
}

// Execute the URL handling logic when the content script loads
handleURL();

// Listen for URL changes (e.g., due to single-page app navigation)
window.addEventListener('popstate', handleURL);
window.addEventListener('hashchange', handleURL);
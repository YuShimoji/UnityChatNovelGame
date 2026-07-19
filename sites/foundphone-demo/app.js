(() => {
  "use strict";

  const REQUIRED_EVENT_NAMES = [
    "demo_started",
    "choice_selected",
    "demo_completed",
    "outbound_store_intent"
  ];

  const elements = {
    prototypeLabel: document.querySelector("#prototype-label"),
    introScreen: document.querySelector("#intro-screen"),
    introEyebrow: document.querySelector("#intro-eyebrow"),
    introTitle: document.querySelector("#intro-title"),
    introSummary: document.querySelector("#intro-summary"),
    startButton: document.querySelector("#start-button"),
    loadStatus: document.querySelector("#load-status"),
    chatScreen: document.querySelector("#chat-screen"),
    threadTitle: document.querySelector("#thread-title"),
    toolbarRestartButton: document.querySelector("#toolbar-restart-button"),
    progressLabel: document.querySelector("#progress-label"),
    routeLabel: document.querySelector("#route-label"),
    progressTrack: document.querySelector(".progress-track"),
    progressFill: document.querySelector("#progress-fill"),
    messageList: document.querySelector("#message-list"),
    choiceRegion: document.querySelector("#choice-region"),
    choicePrompt: document.querySelector("#choice-prompt"),
    choiceList: document.querySelector("#choice-list"),
    advanceButton: document.querySelector("#advance-button"),
    endingScreen: document.querySelector("#ending-screen"),
    endingTitle: document.querySelector("#ending-title"),
    endingBody: document.querySelector("#ending-body"),
    routeOutcome: document.querySelector("#route-outcome"),
    endingRestartButton: document.querySelector("#ending-restart-button"),
    futureReleaseButton: document.querySelector("#future-release-button"),
    futureReleaseNote: document.querySelector("#future-release-note")
  };

  const state = {
    content: null,
    phase: "loading",
    nodeId: null,
    step: 0,
    choiceId: null,
    eventLog: []
  };

  function recordEvent(name, detail = {}) {
    if (!REQUIRED_EVENT_NAMES.includes(name)) {
      throw new Error(`Unknown local event: ${name}`);
    }

    const event = {
      name,
      detail,
      localTimestampMs: Math.round(performance.now())
    };

    state.eventLog.push(event);
    window.dispatchEvent(new CustomEvent("foundphone:event", { detail: event }));
    console.info("[FoundPhone local event]", name, detail);
  }

  function showScreen(screenName) {
    elements.introScreen.hidden = screenName !== "intro";
    elements.chatScreen.hidden = screenName !== "chat";
    elements.endingScreen.hidden = screenName !== "ending";
    document.body.dataset.demoState = screenName;
  }

  function setLoadedContent(content) {
    state.content = content;
    elements.prototypeLabel.textContent = content.meta.contentLabel;
    elements.introEyebrow.textContent = content.intro.eyebrow;
    elements.introTitle.textContent = content.intro.heading;
    elements.introSummary.textContent = content.intro.summary;
    elements.startButton.textContent = content.intro.startLabel;
    elements.startButton.disabled = false;
    elements.loadStatus.textContent = "ローカル fixture の準備ができました。";
    state.phase = "intro";
    showScreen("intro");
  }

  function resetConversation() {
    state.nodeId = state.content.flow.start;
    state.step = 0;
    state.choiceId = null;
    elements.messageList.replaceChildren();
    elements.choiceList.replaceChildren();
    elements.choiceRegion.hidden = true;
    elements.advanceButton.hidden = true;
    elements.futureReleaseNote.hidden = true;
    elements.futureReleaseButton.setAttribute("aria-expanded", "false");
    updateProgress();
  }

  function startDemo() {
    if (!state.content) {
      return;
    }

    resetConversation();
    state.phase = "chat";
    showScreen("chat");
    recordEvent("demo_started", { contentVersion: state.content.meta.version });
    revealCurrentNode();
    elements.threadTitle.focus();
  }

  function updateProgress() {
    const total = state.content?.flow.totalSteps ?? 0;
    const boundedStep = Math.min(state.step, total);
    elements.progressLabel.textContent = `進行 ${boundedStep} / ${total}`;
    elements.progressTrack.setAttribute("aria-valuemax", String(total));
    elements.progressTrack.setAttribute("aria-valuenow", String(boundedStep));
    elements.progressFill.style.width = total === 0 ? "0%" : `${(boundedStep / total) * 100}%`;
    elements.routeLabel.textContent = state.choiceId
      ? `分岐: ${state.choiceId}`
      : "分岐: 未選択";
  }

  function participantFor(speakerId) {
    return state.content.participants[speakerId] ?? {
      label: speakerId,
      role: "system"
    };
  }

  function appendMessage(speakerId, text) {
    const participant = participantFor(speakerId);
    const item = document.createElement("li");
    item.className = `message message--${participant.role}`;
    item.setAttribute("aria-label", `${participant.label}からのメッセージ`);

    const speaker = document.createElement("p");
    speaker.className = "message-speaker";
    speaker.textContent = participant.label;

    const bubble = document.createElement("p");
    bubble.className = "message-bubble";
    bubble.textContent = text;

    item.append(speaker, bubble);
    elements.messageList.append(item);
    item.scrollIntoView({ block: "nearest" });
  }

  function configureAdvanceButton(nextNodeId) {
    const nextNode = state.content.flow.nodes[nextNodeId];
    elements.advanceButton.textContent = nextNode?.type === "choice"
      ? "返信を選ぶ"
      : nextNode?.type === "ending"
        ? "結果を見る"
        : "続ける";
    elements.advanceButton.hidden = false;
    elements.advanceButton.focus();
  }

  function revealMessage(node) {
    appendMessage(node.speaker, node.text);
    state.step += 1;
    state.nodeId = node.next;
    updateProgress();
    configureAdvanceButton(node.next);
  }

  function revealChoice(node) {
    state.phase = "choice";
    elements.advanceButton.hidden = true;
    elements.choicePrompt.textContent = node.prompt;
    elements.choiceList.replaceChildren();

    for (const option of node.options) {
      const button = document.createElement("button");
      button.className = "choice-button";
      button.type = "button";
      button.textContent = option.label;
      button.dataset.choiceId = option.id;
      button.addEventListener("click", () => selectChoice(option));
      elements.choiceList.append(button);
    }

    elements.choiceRegion.hidden = false;
    elements.choiceList.querySelector("button")?.focus();
  }

  function selectChoice(option) {
    state.choiceId = option.id;
    state.step += 1;
    state.phase = "chat";
    appendMessage("player", option.reply);
    recordEvent("choice_selected", {
      choiceId: option.id,
      nodeId: state.nodeId
    });
    state.nodeId = option.next;
    elements.choiceRegion.hidden = true;
    updateProgress();
    configureAdvanceButton(option.next);
  }

  function revealEnding(node) {
    state.step += 1;
    state.phase = "ending";
    updateProgress();
    elements.endingTitle.textContent = node.heading;
    elements.endingBody.textContent = node.body;
    elements.routeOutcome.textContent = node.outcomes[state.choiceId] ?? node.defaultOutcome;
    showScreen("ending");
    recordEvent("demo_completed", {
      choiceId: state.choiceId,
      steps: state.step
    });
    elements.endingTitle.focus();
  }

  function revealCurrentNode() {
    const node = state.content.flow.nodes[state.nodeId];
    if (!node) {
      throw new Error(`Demo node was not found: ${state.nodeId}`);
    }

    if (node.type === "message") {
      revealMessage(node);
      return;
    }

    if (node.type === "choice") {
      revealChoice(node);
      return;
    }

    if (node.type === "ending") {
      revealEnding(node);
      return;
    }

    throw new Error(`Unsupported demo node type: ${node.type}`);
  }

  function showFutureReleaseNote() {
    const isExpanded = elements.futureReleaseButton.getAttribute("aria-expanded") === "true";
    const nextExpanded = !isExpanded;
    elements.futureReleaseButton.setAttribute("aria-expanded", String(nextExpanded));
    elements.futureReleaseNote.hidden = !nextExpanded;

    if (nextExpanded) {
      recordEvent("outbound_store_intent", {
        destination: null,
        mode: "local_note_only"
      });
    }
  }

  function exposeTestState() {
    window.FoundPhoneDemo = Object.freeze({
      getState: () => ({
        phase: state.phase,
        nodeId: state.nodeId,
        step: state.step,
        choiceId: state.choiceId,
        messageCount: elements.messageList.children.length,
        eventNames: state.eventLog.map((event) => event.name)
      }),
      requiredEventNames: [...REQUIRED_EVENT_NAMES],
      start: startDemo,
      restart: startDemo
    });
  }

  async function initialize() {
    try {
      const response = await fetch("./content/demo.json", { cache: "no-store" });
      if (!response.ok) {
        throw new Error(`Content request failed with HTTP ${response.status}`);
      }

      const content = await response.json();
      setLoadedContent(content);
    } catch (error) {
      state.phase = "error";
      elements.loadStatus.classList.add("is-error");
      elements.loadStatus.textContent = `デモを読み込めませんでした: ${error.message}`;
      console.error(error);
    }
  }

  elements.startButton.addEventListener("click", startDemo);
  elements.toolbarRestartButton.addEventListener("click", startDemo);
  elements.advanceButton.addEventListener("click", revealCurrentNode);
  elements.endingRestartButton.addEventListener("click", startDemo);
  elements.futureReleaseButton.addEventListener("click", showFutureReleaseNote);

  exposeTestState();
  initialize();
})();

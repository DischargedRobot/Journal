import Stack from "@mui/material/Stack"
import Box from "@mui/material/Box"
import {
	Children,
	cloneElement,
	createContext,
	isValidElement,
	ReactElement,
	ReactNode,
	useContext,
	useState,
} from "react"
import { Label, Step, StepContent, StepHeader } from "./Step"

interface Props {
	children?: ReactNode
}

type WizardContextValue = {
	currentStep: number | string
	setCurrentStep: (step: number | string) => void
}

const WizardContext = createContext<WizardContextValue | null>(null)

export const useWizard = () => {
	const ctx = useContext(WizardContext)
	if (!ctx) {
		throw new Error("useWizard must be used inside <Wizard>")
	}
	return ctx
}

const getStepChildren = (step: ReactElement<{ children?: ReactNode }>) =>
	Children.toArray(step.props.children).filter(isValidElement)

const Wizard = (props: Props) => {
	const { children } = props

	const steps = Children.toArray(children).filter(
		(
			child,
		): child is ReactElement<{
			children?: ReactNode
			stepId?: number | string
		}> => isValidElement(child) && child.type === Step,
	)

	const initialStepId = steps[0]?.props.stepId ?? 1
	const [currentStep, setCurrentStep] = useState<number | string>(
		initialStepId,
	)

	const wizardContextValue: WizardContextValue = {
		currentStep,
		setCurrentStep,
	}

	const headers: ReactElement[] = []
	const contents: ReactElement[] = []

	steps.forEach((step, index) => {
		const stepId = step.props.stepId ?? index + 1
		const header = getStepChildren(step).find(
			(child) => child.type === StepHeader,
		)
		const content = getStepChildren(step).find(
			(child) => child.type === StepContent,
		)

		if (header) {
			headers.push(
				cloneElement(step, { key: `header-${stepId}`, stepId }, header),
			)
		}
		if (content) {
			contents.push(
				cloneElement(
					step,
					{ key: `content-${stepId}`, stepId },
					content,
				),
			)
		}
	})

	return (
		<WizardContext.Provider value={wizardContextValue}>
			<Box className="flex flex-col w-full">
				<Stack
					direction="row"
					spacing={2}
					className="justify-between w-full p-4"
				>
					{headers}
				</Stack>
				<Box className="grid flex-1">{contents}</Box>
			</Box>
		</WizardContext.Provider>
	)
}

Wizard.Label = Label
Wizard.StepHeader = StepHeader
Wizard.Step = Step
Wizard.StepContent = StepContent

export default Wizard
export { Label, Step, StepContent, StepHeader }

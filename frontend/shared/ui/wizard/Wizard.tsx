import Stack from "@mui/material/Stack"
import Box from "@mui/material/Box"
import {
	Children,
	cloneElement,
	createContext,
	isValidElement,
	memo,
	ReactElement,
	ReactNode,
	useContext,
} from "react"
import Label from "./Label"
import Step from "./Step"
import StepContent from "./StepContent"
import StepHeader from "./StepHeader"

type WizardContextValue = {
	currentStep: number | string
	onStepChange: (step: number | string) => void
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

interface Props {
	children?: ReactNode
	currentStep: number | string
	onStepChange: (step: number | string) => void
}

const WizardBase = (props: Props) => {
	const { children, currentStep, onStepChange } = props

	// шаг\этапы визарда
	const steps = Children.toArray(children).filter(
		(
			child,
		): child is ReactElement<{
			children?: ReactNode
			stepId?: number | string
		}> => isValidElement(child) && child.type === Step,
	)

	const wizardContextValue: WizardContextValue = {
		currentStep,
		onStepChange,
	}

	const headers: ReactElement[] = []
	const contents: ReactElement[] = []

	// Собираем в шаги заголовки и контент чтобы потом отрендарить в блоках отдельных друг от друга
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
			<Box className="flex flex-col w-full mt-1 gap-2">
				<Box
					className="grid grid-rows-1 justify-between items-start w-full gap-2"
					sx={{
						gridTemplateColumns: `repeat(${headers.length}, 1fr)`,
					}}
				>
					{headers}
				</Box>
				<Box className="grid flex-1">{contents}</Box>
			</Box>
		</WizardContext.Provider>
	)
}

const Wizard = Object.assign(memo(WizardBase), {
	Label,
	Step,
	StepContent,
	StepHeader,
})

export default Wizard
